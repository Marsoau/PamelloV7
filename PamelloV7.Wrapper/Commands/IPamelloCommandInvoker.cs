namespace PamelloV7.Wrapper.Commands;

public interface IPamelloCommandInvoker
{
    public Task<string> ExecuteCommandPathAsync(string commandPath);
    public Task<TType> ExecuteCommandPathAsync<TType>(string commandPath);
}
