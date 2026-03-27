using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiProyecto.Web;
using MiProyecto.Web.Services;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 🔥 Handler JWT
builder.Services.AddScoped<JwtAuthorizationHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

// HttpClient solo para login (sin JWT)
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!);
});

// AuthService usando AuthClient
builder.Services.AddScoped<IAuthService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var js = sp.GetRequiredService<IJSRuntime>();
    var authProvider = sp.GetRequiredService<CustomAuthenticationStateProvider>();
    return new MiProyecto.Web.Services.AuthService(factory.CreateClient("AuthClient"), js, authProvider);
});

// HttpClient central con JWT handler para toda la API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!);
})
.AddHttpMessageHandler<JwtAuthorizationHandler>();

// 🔥 Servicios de la app
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<OfflineQueueService>();
builder.Services.AddScoped<OfflineCacheService>();
builder.Services.AddScoped<NetworkService>();


await builder.Build().RunAsync();