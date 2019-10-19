using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Alexa.NET.SkillFlow.Interpreter.Tests
{
    public class InterpreterTests
    {
        [Fact]
        public async Task NullStreamThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SkillFlowInterpreter().Interpret((Stream)null));
        }

        [Fact]
        public async Task NullPipeThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SkillFlowInterpreter().Interpret((PipeReader)null).AsTask());
        }

        [Fact]
        public async Task NullStringThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => new SkillFlowInterpreter().Interpret((string)null));
        }

        [Fact]
        public async Task EmptyStringReturnsEmptySkillFlow()
        {
            var result = await new SkillFlowInterpreter().Interpret(string.Empty);
            Assert.NotNull(result);
            Assert.Empty(result.Scenes);
        }

        [Fact]
        public async Task HandlesNewLine()
        {
            var story = await new SkillFlowInterpreter().Interpret("@test" + Environment.NewLine);
            var scene = Assert.Single(story.Scenes);
            Assert.Equal("test", scene.Key);
            Assert.Equal("test", scene.Value.Name);
        }

        [Fact]
        public async Task AddOccursOnCorrectTab()
        {
            var story = await new SkillFlowInterpreter().Interpret("@test");
            var scene = Assert.Single(story.Scenes);
            Assert.Equal("test", scene.Key);
            Assert.Equal("test", scene.Value.Name);
        }

        [Fact]
        public async Task ThrowsOnInvalidSkillFlow()
        {
            var ex = await Assert.ThrowsAsync<InvalidSkillFlowDefinitionException>(() => new SkillFlowInterpreter().Interpret($"@scene test {Environment.NewLine} ~"));
            Assert.Equal(2, ex.LineNumber);
        }

        [Fact]
        public async Task MultilineAddsToCorrectComponent()
        {
            var interpreter = new SkillFlowInterpreter();
            var story = await interpreter.Interpret(string.Join(Environment.NewLine, "@scene test",
                "\t*say","\t\twibble","\t\t||","\t\ttest"));
            var scene = Assert.Single(story.Scenes).Value;
            Assert.Null(scene.Reprompt);
            Assert.Null(scene.Recap);
            Assert.NotNull(scene.Say);
            Assert.Equal(2,scene.Say.Content.Count);
        }

        [Fact]
        public async Task InterpretsBaseClassCorrectly()
        {
            var interpreter = new SkillFlowInterpreter();
            await interpreter.Interpret(string.Join(Environment.NewLine, "@scene test",
                "\t*then", "\t\t-> thing"));
        }

        [Fact]
        public async Task ThrowsWhenUnableToFindInterpreters()
        {
            var interpreter = new SkillFlowInterpreter();
            interpreter.TypedInterpreters.Remove(typeof(Story));
            var exception = await Assert.ThrowsAsync<InvalidSkillFlowDefinitionException>(() => interpreter.Interpret("@scene test"));
            Assert.StartsWith("1: Unknown definition for Story",exception.Message);
        }

        [Fact]
        public async Task ThrowOnBadGroupEndIndent()
        {
            var interpreter = new SkillFlowInterpreter(new SkillFlowInterpretationOptions { LineEnding = "\n" });
            var exception = await Assert.ThrowsAsync<InvalidSkillFlowDefinitionException>(() => interpreter.Interpret("@test\n\t*then\n\t\tif !test {\n\t\t\tflag test\n@test2"));
            Assert.StartsWith("5: Unclosed group", exception.Message);
        }

        [Fact]
        public async Task CommentsAreAttachedToComponent()
        {
            var interpreter = new SkillFlowInterpreter(new SkillFlowInterpretationOptions { LineEnding = "\n" });
            var result = await interpreter.Interpret("//This is a comment\n//This is another comment\n@test");
            var scene = result.Scenes.First().Value;
            Assert.Equal(2,scene.Comments.Length);
            Assert.Equal("This is a comment",scene.Comments.First());
            Assert.Equal("This is another comment", scene.Comments.Skip(1).First());
        }

        [Fact]
        public async Task CommentsClearedOnAttachment()
        {
            var interpreter = new SkillFlowInterpreter(new SkillFlowInterpretationOptions { LineEnding = "\n" });
            var result = await interpreter.Interpret("//This is a comment\n//This is another comment\n@test\n\t//This is for say\n\t*say\n\t\tTest say statement");
            var scene = result.Scenes.First().Value;
            var say = scene.Say;
            var comment = Assert.Single(say.Comments);
            Assert.Equal("This is for say",comment);
        }
    }
}
