# Alexa.NET.SkillFlow.Interpreter
A small library interpreting SkillFlow text into a strongly typed model

The model that it creates can be found at [Alexa.NET.SkillFlow](https://github.com/stoiveyp/Alexa.NET.SkillFlow)

## How to create a model from a story file
```csharp
var interpreter = new SkillFlowInterpreter();
Story story = null;
using (var stream = File.OpenRead("story.abc"))
{
    story = await interpreter.Interpret(stream);
}
