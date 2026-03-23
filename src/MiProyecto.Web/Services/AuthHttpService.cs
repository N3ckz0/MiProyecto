using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace MiProyecto.Web.Services;

public class AuthHttpService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public AuthHttpService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    private async Task AddAuthHeader()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        await AddAuthHeader();
        return await _http.GetAsync(url);
    }
}