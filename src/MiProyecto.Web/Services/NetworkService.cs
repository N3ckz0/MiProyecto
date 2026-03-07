using Microsoft.JSInterop;

public class NetworkService
{
    private readonly IJSRuntime _js;

    public NetworkService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> IsOnline()
    {
        return await _js.InvokeAsync<bool>("networkHelper.isOnline");
    }
}