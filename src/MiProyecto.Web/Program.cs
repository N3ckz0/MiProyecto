using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiProyecto.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient{ BaseAddress = new Uri(builder.Configuration["ApiUrl"]!) });
builder.Services.AddScoped<NetworkService>();
builder.Services.AddScoped<OfflineCacheService>();
builder.Services.AddScoped<OfflineQueueService>();
builder.Services.AddScoped<ProductoService>();
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
