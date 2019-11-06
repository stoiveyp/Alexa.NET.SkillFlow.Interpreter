using Alexa.NET.SkillFlow.Instructions;
using Alexa.NET.SkillFlow.Interpreter;
using Xunit;

namespace Alexa.NET.SkillFlow.Tests
{
    public class SetTests
    {
        [Fact]
        public void CorrectlyIdentifiesText()
        {
            var interpreter = new SetInterpreter();
            var context = new SkillFlowInterpretationContext(new SkillFlowInterpretationOptions());
            context.Components.Push(new SceneInstructions());
            Assert.True(interpreter.CanInterpret("set test to 3", context));
        }

        [Fact]
        public void CorrectlyIdentifiesVariable()
        {
            var interpreter = new SetInterpreter();
            var context = new SkillFlowInterpretationContext(new SkillFlowInterpretationOptions());
            context.Components.Push(new SceneInstructions());
            Assert.True(interpreter.CanInterpret("set test as testSlot", context));
            var test = interpreter.Interpret("set test as testSlot",context);
        }

        [Fact]
        public void CreatesComponentCorrectly()
        {
            var interpreter = new SetInterpreter();
            var result = interpreter.Interpret("set test to 3",
                new SkillFlowInterpretationContext(new SkillFlowInterpretationOptions()));
            var instruction = Assert.IsType<Set>(result.Component);
            Assert.Equal("test", instruction.Variable);
            Assert.Equal(3.ToString(), instruction.Value);
        }

        [Fact]
        public void AddsCorrectly()
        {
            var instruction = new Set("test",3);
            var instructions = new SceneInstructions();
            instructions.Add(instruction);
            var result = Assert.Single(instructions.Instructions);
            Assert.Equal(instruction, result);
        }
    }
}
