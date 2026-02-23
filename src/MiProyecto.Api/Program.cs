using MiProyecto.Aplicacion.Clientes;
using MiProyecto.Aplicacion.Productos;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy =>
        {
            policy.WithOrigins("http://localhost:5028")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");

app.MapControllers();

app.Run();