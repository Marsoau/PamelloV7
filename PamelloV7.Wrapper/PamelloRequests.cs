using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Wrapper;

public class PamelloRequests
{
    private readonly HttpClient _http;
    
    public string BaseUrl {
        get => _http.BaseAddress?.ToString() ?? throw new Exception("Base URL not set in PamelloRequests");
        set => _http.BaseAddress = new Uri(value);
    }
    
    public Guid? Token { get; set; }
    
    public PamelloRequests() {
        _http = new HttpClient();
    }
    public PamelloRequests(IServiceProvider services) {
        _http = services.GetRequiredService<IHttpClientFactory>().CreateClient();
    }

    public async Task<TType> GetFromJsonAsync<TType>([StringSyntax("Uri")] string url, bool requireUser = false) {
        var response = await GetAsync(url, requireUser);
        return await response.Content.ReadFromJsonAsync<TType>() ?? throw new PamelloException($"Cannot read response as {typeof(TType).Name}");
    }
    public async Task<HttpResponseMessage> GetAsync([StringSyntax("Uri")] string url, bool requireUser = false) {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        if (Token is not null) request.Headers.Add("user", Token.ToString());
        else if (requireUser) throw new PamelloException("User token is not provided");
        
        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode) {
            throw new PamelloException(await response.Content.ReadAsStringAsync());
        }
        
        return response;
    }
    
    public async Task<bool> PingAsync() => (await GetAsync("Ping")).IsSuccessStatusCode;
    
    public async Task<string> ExecuteCommandAsync(string commandPath) => await (await GetAsync($"Command/{commandPath}")).Content.ReadAsStringAsync();
    public async Task<TType> ExecuteCommandAsync<TType>(string commandPath) => await GetFromJsonAsync<TType>($"Command/{commandPath}");

    public Task<List<PamelloEntityDto>> GetEntitiesAsync(string fullQuery)
        => GetEntitiesAsync<PamelloEntityDto>(fullQuery);
    public async Task<List<TPamelloDto>> GetEntitiesAsync<TPamelloDto>(string fullQuery)
        where TPamelloDto : PamelloEntityDto
    {
        var type = typeof(TPamelloDto);
        
        //TODO check the type default provider and use it
        
        return await GetFromJsonAsync<List<TPamelloDto>>($"Data/{fullQuery}");
    }
}
