using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper.Commands;
using PamelloV7.Wrapper.Config;
using PamelloV7.Wrapper.Exceptions;

namespace PamelloV7.Wrapper;

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

    public async Task<TType> GetFromJsonAsync<TType>([StringSyntax("Uri")] string url, bool requireUser = false) {
        var response = await GetAsync(url, requireUser);
        
        var content = await response.Content.ReadAsStringAsync();
        Debug.WriteLine($"Content to read as json: {content}");
        
        var o = content.Length > 0 ? JsonSerializer.Deserialize<TType>(content) : default;
        
        return o ?? throw new PamelloException($"Cannot read response as {typeof(TType).Name}");
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
    
    public async Task<string> ExecuteCommandAsync(string commandPath) => await (await GetAsync($"Command/{commandPath}")).Content.ReadAsStringAsync();
    public async Task<TType> ExecuteCommandAsync<TType>(string commandPath) => await GetFromJsonAsync<TType>($"Command/{commandPath}");

    public Task<List<PamelloEntityDto>> GetEntitiesAsync(string fullQuery)
        => GetEntitiesAsync<PamelloEntityDto>(fullQuery);
    public async Task<List<TPamelloDto>> GetEntitiesAsync<TPamelloDto>(string fullQuery)
        where TPamelloDto : PamelloEntityDto
    {
        return await GetFromJsonAsync<List<TPamelloDto>>($"Data/{fullQuery}");
    }
}
