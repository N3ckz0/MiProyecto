using Microsoft.JSInterop;

public class NetworkService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<NetworkService>? _objRef;
    public event Action<bool>? OnStatusChanged;

    public NetworkService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        _objRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("networkService.registerStatusChange", _objRef);
    }

    public async Task<bool> IsOnline()
    {
        return await _js.InvokeAsync<bool>("networkService.isOnline");
    }

    [JSInvokable]
    public void UpdateStatus(bool isOnline)
    {
        OnStatusChanged?.Invoke(isOnline);
    }

    public async ValueTask DisposeAsync()
    {
        if (_objRef != null)
        {
            await _js.InvokeVoidAsync("networkService.unregisterStatusChange", _objRef);
            _objRef.Dispose();
        }
    }
}