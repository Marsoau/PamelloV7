using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Exceptions;

namespace PamelloV7.Wrapper.Requests;

public class PamelloRequests : IPamelloCommandInvoker
{
    private readonly PamelloClientConfig _config;
    
    private readonly HttpClient _http;
    
    public PamelloRequests(PamelloClientConfig config) {
        _config = config;
        
        _http = new HttpClient();
    }
    public PamelloRequests(PamelloClientConfig config, IServiceProvider services) {
        _config = config;
        
        _http = services.GetRequiredService<IHttpClientFactory>().CreateClient();
    }

    public async Task<TType> GetFromJsonAsync<TType>([StringSyntax("Uri")] string url, bool requireUser = false)
        => ((TType?)await GetFromJsonAsync(typeof(TType), url, requireUser))!;
    public async Task<object?> GetFromJsonAsync(Type type, [StringSyntax("Uri")] string url, bool requireUser = false) {
        var response = await GetAsync(url, requireUser);
        
        var content = await response.Content.ReadAsStringAsync();
        Debug.WriteLine($"Content to read as json: {content}");
        
        var o = content.Length > 0 ? JsonSerializer.Deserialize(content, type) : null;
        
        return o ?? throw new PamelloException($"Cannot read response as {type.Name}");
    }
    public async Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string url, bool requireUser = false) {
        if (_config.BaseUrl is null) throw new PamelloException("BaseUrl of PamelloClientConfig wasnt set");
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.BaseUrl}/{url}");
        
        if (_config.Token is not null) request.Headers.Add("user", _config.Token.ToString());
        else if (requireUser) throw new PamelloException("User token is not provided");
        
        var response = await _http.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.BadGateway) {
            throw new NotConnectedPamelloException($"\"BadGateway\" trying to request: {request.RequestUri}");
        }
        if (!response.IsSuccessStatusCode) {
            throw new RemotePamelloException(await response.Content.ReadAsStringAsync());
        }
        
        return response;
    }
    
    public async Task<bool> PingAsync() {
        try {
            return (await GetAsync("Ping")).IsSuccessStatusCode;
        }
        catch (NotConnectedPamelloException) {
            return false;
        }
    }
    
    public async Task<string> ExecuteCommandPathAsync(string commandPath) => await (await GetAsync($"Command/{commandPath}")).Content.ReadAsStringAsync();
    public async Task<TType> ExecuteCommandPathAsync<TType>(string commandPath) => await GetFromJsonAsync<TType>($"Command/{commandPath}");

    public Task<List<PamelloEntityDto>> GetEntitiesAsync(string fullQuery)
        => GetEntitiesAsync<PamelloEntityDto>(fullQuery);
    public async Task<List<TPamelloDto>> GetEntitiesAsync<TPamelloDto>(string query)
        where TPamelloDto : PamelloEntityDto
        => (await GetEntitiesAsync(typeof(TPamelloDto), query)).OfType<TPamelloDto>().ToList();
    
    public async Task<List<PamelloEntityDto>> GetEntitiesAsync(Type type, string query) {
        if (!type.IsAssignableTo(typeof(PamelloEntityDto))) throw new ArgumentException("Type must be assignable to PamelloEntityDto");
        return (await GetFromJsonAsync(typeof(List<>).MakeGenericType(type), $"Data/{query}") as IList)?.OfType<PamelloEntityDto>().ToList() ?? [];
    }
}
