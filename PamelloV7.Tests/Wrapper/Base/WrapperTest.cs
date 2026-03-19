using PamelloV7.Wrapper;

namespace PamelloV7.Tests.Wrapper.Base;

public abstract class WrapperTest
{
    protected readonly PamelloClient _client;
    
    protected WrapperTest() {
        _client = new PamelloClient();
    }
    
    [Fact]
    protected async Task ConnectionTest() {
        await _client.ConnectAsync("http://127.0.0.1:41630");
        Assert.True(_client.IsConnected);
        await _client.AuthorizeAsync(Guid.Parse("9a40ad25-7e80-43c1-bdd9-a7a84218db5d"));
        Assert.True(_client.IsAuthorized);
    }
}
