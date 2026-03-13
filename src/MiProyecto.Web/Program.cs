using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiProyecto.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient{ BaseAddress = new Uri(builder.Configuration["ApiUrl"]!) });
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
