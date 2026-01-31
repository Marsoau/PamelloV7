using PamelloV7.Server.Model.AudioOld.Modules.Basic;
using Xunit.Abstractions;

namespace PamelloV7.Tests.AudioOld;

public class AudioChoiceTests : AudioTestWithModel
{
    private readonly ITestOutputHelper _testOutputHelper;
    
    public readonly AudioChoise Choice;

    public AudioChoiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        Choice = _model.AddModule(new AudioChoise(_model));
    }

    [Fact]
    public void CorrectlyCreated()
    {
        Assert.Empty(Choice.Inputs);
        Assert.NotNull(Choice.Output);
    }
}