namespace PamelloV7.Wrapper.Commands;

public interface IPamelloCommandInvoker
{
    public Task<string> ExecuteCommandAsync(string commandPath);
    public Task<TType> ExecuteCommandAsync<TType>(string commandPath);
}
