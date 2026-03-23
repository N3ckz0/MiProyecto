using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiProyecto.Web;
using MiProyecto.Web.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient base para llamadas a la API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiUrl"]!) });

// Registrar servicios con implementaciones concretas
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHttpService>();
builder.Services.AddScoped<NetworkService>();
builder.Services.AddScoped<OfflineCacheService>();
builder.Services.AddScoped<OfflineQueueService>();
builder.Services.AddScoped<ProductoService>();

await builder.Build().RunAsync();