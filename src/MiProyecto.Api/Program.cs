using MiProyecto.Aplicacion.Saludo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ISaludoService, SaludoService>();

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