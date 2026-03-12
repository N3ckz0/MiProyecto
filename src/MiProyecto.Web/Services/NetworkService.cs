using Microsoft.JSInterop;

public class NetworkService
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<NetworkService>? _objRef;

    public NetworkService(IJSRuntime js)
    {
        _js = js;
    }

    public event Func<Task>? OnOnlineEvent;
    public event Func<Task>? OnOfflineEvent;

    public async Task<bool> IsOnline()
    {
        return await _js.InvokeAsync<bool>("networkHelper.isOnline");
    }

    public async Task RegisterConnectionEvents()
    {
        _objRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("networkHelper.registerConnectionEvents", _objRef);
    }

    [JSInvokable]
    public async Task OnOnline()
    {
        if (OnOnlineEvent != null)
            await OnOnlineEvent.Invoke();
    }

    [JSInvokable]
    public async Task OnOffline()
    {
        if (OnOfflineEvent != null)
            await OnOfflineEvent.Invoke();
    }
}