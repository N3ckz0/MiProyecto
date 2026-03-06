using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MiProyecto.Web;
using TG.Blazor.IndexedDB;
using MiProyecto.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["ApiUrl"]!)
    });

builder.Services.AddScoped<ProductoOfflineService>();

builder.Services.AddIndexedDB(dbStore =>
{
    dbStore.DbName = "MiProyectoDB";
    dbStore.Version = 4;

    dbStore.Stores.Add(new StoreSchema
    {
        Name = "productos",
        PrimaryKey = new IndexSpec
        {
            Name = "Id_producto",
            KeyPath = "Id_producto",
            Auto = false
        }
    });
});


builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
