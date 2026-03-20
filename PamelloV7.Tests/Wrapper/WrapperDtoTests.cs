using PamelloV7.Tests.Wrapper.Base;
using PamelloV7.Wrapper.Extensions;
using Xunit.Abstractions;

namespace PamelloV7.Tests.Wrapper;

public class WrapperDtoTests : WrapperTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    public WrapperDtoTests(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task RequestUserTest() {
        await ConnectionTest();
        Assert.NotNull(_client.User);

        await _client.User.FavoriteSongs.LoadAsync();

        _testOutputHelper.WriteLine(_client.User.FavoriteSongs.Count().ToString());
    }
    
    [Fact]
    public async Task SongAdderTest() {
        await ConnectionTest();

        await _client.RequiredUser.FavoriteSongs.LoadAsync();
        var song = _client.RequiredUser.FavoriteSongs.First();
        var addedBy = await song.AddedBy.LoadAsync();
        
        Assert.NotNull(addedBy);
        _testOutputHelper.WriteLine(addedBy.ToString());
    }
    
    [Fact]
    public async Task CurrentSongTest() {
        await ConnectionTest();
        
        Assert.NotNull(_client.RequiredUser.SelectedPlayer);

        var player = await _client.RequiredUser.SelectedPlayer.LoadAsync();
        if (player is null) {
            _testOutputHelper.WriteLine("No player selected");
            return;
        }
        
        Assert.NotNull(player.Queue.CurrentSong);
        
        var song = await player.Queue.CurrentSong.LoadAsync();
        if (song is null) {
            _testOutputHelper.WriteLine("No current song");
            return;
        }
        
        _testOutputHelper.WriteLine(song.ToString());
    }
}
