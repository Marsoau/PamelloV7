namespace PamelloV7.Wrapper.ClientExample.Examples;

public class SimpleApp
{
    public static async Task Run() {
        var client = new PamelloClient();
        
        await client.ConnectAsync("http://localhost:41630");
        client.AuthorizeAsync();
    }
}
