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

        await _client.User.FavoriteSongsIds.LoadAsync();

        _testOutputHelper.WriteLine(_client.User.FavoriteSongsIds.Count().ToString());
    }
}
