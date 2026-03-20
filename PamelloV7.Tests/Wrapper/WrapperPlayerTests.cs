using PamelloV7.Core.Audio;
using PamelloV7.Tests.Wrapper.Base;
using PamelloV7.Wrapper.Extensions;
using Xunit.Abstractions;

namespace PamelloV7.Tests.Wrapper;

public class WrapperPlayerTests : WrapperTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    
    public WrapperPlayerTests(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task PlayerTimeTest() {
        await ConnectionTest();
        
        Assert.NotNull(_client.RequiredUser.SelectedPlayer);
        
        var player = await _client.RequiredUser.SelectedPlayer.LoadAsync();
        if (player is null) {
            _testOutputHelper.WriteLine("No player selected");
            return;
        }

        _client.Events.Watch(() => {
            var time = new AudioTime(player.Queue.CurrentSongTimePassed);
            _testOutputHelper.WriteLine(time.ToString());
        }, () => [player]);
        
        _testOutputHelper.WriteLine("y");
        
        await Task.Delay(-1);
    }
    
    [Fact]
    public async Task PlayerQueueTest() {
        await ConnectionTest();
        
        Assert.NotNull(_client.RequiredUser.SelectedPlayer);
        
        var player = await _client.RequiredUser.SelectedPlayer.LoadAsync();
        if (player is null) {
            _testOutputHelper.WriteLine("No player selected");
            return;
        }

        _client.Events.Watch(() => {
            _testOutputHelper.WriteLine("- - - - - - - - -");
            _testOutputHelper.WriteLine(string.Join("\n", player.Queue.Entries.Select(entry => entry.Song.LoadAsync().Result?.ToString())));
        }, () => [player]);
        
        _testOutputHelper.WriteLine("y");
        
        await Task.Delay(-1);
    }
}
