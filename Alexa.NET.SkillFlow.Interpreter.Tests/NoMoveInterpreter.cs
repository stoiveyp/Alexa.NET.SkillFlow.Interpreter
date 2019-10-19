using Alexa.NET.SkillFlow.Interpreter;

namespace Alexa.NET.SkillFlow.Interpreter.Tests
{
    public class NoMoveInterpreter : ISkillFlowInterpreter
    {
        public bool CanInterpret(string candidate, SkillFlowInterpretationContext context)
        {
            return candidate.StartsWith('~');
        }

        public InterpreterResult Interpret(string candidate, SkillFlowInterpretationContext context)
        {
            return InterpreterResult.Empty;
        }
    }
}