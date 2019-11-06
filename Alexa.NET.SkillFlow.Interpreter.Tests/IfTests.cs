using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.SkillFlow.Conditions;
using Alexa.NET.SkillFlow.Instructions;
using Alexa.NET.SkillFlow.Interpreter;
using Xunit;

namespace Alexa.NET.SkillFlow.Tests
{
    public class IfTests
    {
        [Fact]
        public void CorrectlyIdentifiesText()
        {
            var interpreter = new IfInterpreter();
            var context = new SkillFlowInterpretationContext(new SkillFlowInterpretationOptions());
            context.Components.Push(new SceneInstructions());
            Assert.True(interpreter.CanInterpret("if test {", context));
        }

        [Fact]
        public void CorrectlyIdentifiesFalseText()
        {
            var interpreter = new IfInterpreter();
            Assert.False(interpreter.CanInterpret("iffy", new SkillFlowInterpretationContext(new SkillFlowInterpretationOptions())));
        }

        [Fact]
        public void AddsCorrectly()
        {
            var hear = new If(new ValueWrapper(new True()));
            var instructions = new SceneInstructions();
            instructions.Add(hear);
            var result = Assert.Single(instructions.Instructions);
            Assert.Equal(hear, result);
        }

        [Fact]
        public async Task VariableAndStringCorrectly()
        {
            var interpreter = new SkillFlowInterpreter(new SkillFlowInterpretationOptions { LineEnding = "\n" });
            var result = await interpreter.Interpret("@test\n\t*then\n\t\tif bottles == 'mage' {\n\t\t\tflag test\n\t\t\t}");
            var instruction = Assert.Single(result.Scenes.First().Value.Instructions.Instructions);
            var ifInstruction = Assert.IsType<If>(instruction);
            var equal = Assert.IsType<Equal>(ifInstruction.Condition);
            var variable = Assert.IsType<Variable>(equal.Left);
            var text = Assert.IsType<LiteralValue>(equal.Right);
            Assert.Equal("bottles", variable.Name);
            Assert.Equal("mage", text.Value);
        }

        [Fact]
        public async Task CreatesConditionCorrectly()
        {
            var interpreter = new SkillFlowInterpreter(new SkillFlowInterpretationOptions{LineEnding = "\n"});
            var result = await interpreter.Interpret("@test\n\t*then\n\t\tif !test {\n\t\t\tflag test\n\t\t\t}");
            var instruction = Assert.Single(result.Scenes.First().Value.Instructions.Instructions);
            var ifInstruction = Assert.IsType<If>(instruction);
            var not = Assert.IsType<Not>(ifInstruction.Condition);
            var literal = Assert.IsType<Variable>(not.Condition);
            Assert.Equal("test",literal.Name);
        }
    }
}
