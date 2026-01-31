using PamelloV7.Server.Model.AudioOld;
using PamelloV7.Server.Model.AudioOld.Modules.Inputs;
using Xunit.Abstractions;

namespace PamelloV7.Tests.AudioOld;

public class AudioSilenceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AudioSilenceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Pull()
    {
        var buffer = new byte[] { 4, 4, 4, 4 };
        
        var model = new AudioModel();
        var silence = model.AddModule(new AudioSilence(model));

        await silence.Output.Pull(buffer, true, CancellationToken.None);
        
        Assert.Equal([0, 0, 0, 0], buffer);
    }
}