using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiProyecto.Web;
using MiProyecto.Web.Services;
using System.Net.Http;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
    
builder.Services.AddScoped<JwtAuthorizationHandler>();

// HttpClient base para llamadas a la API
builder.Services.AddHttpClient<IAuthService, MiProyecto.Web.Services.AuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!);
});

builder.Services.AddHttpClient<ProductoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!);
})
.AddHttpMessageHandler<JwtAuthorizationHandler>();

// Registrar servicios con implementaciones concretas
builder.Services.AddScoped<NetworkService>();
builder.Services.AddScoped<OfflineCacheService>();
builder.Services.AddScoped<OfflineQueueService>();

await builder.Build().RunAsync();