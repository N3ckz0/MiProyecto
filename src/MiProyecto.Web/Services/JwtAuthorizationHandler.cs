using System.Net.Http.Headers;
using Microsoft.JSInterop;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public JwtAuthorizationHandler(IJSRuntime js)
    {
        _js = js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine("🔥 Handler ejecutándose");

        var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        Console.WriteLine($"TOKEN: {token}");

        return await base.SendAsync(request, cancellationToken);
    }
}