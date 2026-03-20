using MiProyecto.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace MiProyecto.Web.Services;

public class TestOffline
{
    private readonly ProductoService _productoService;
    private readonly OfflineQueueService _queueService;
    private readonly NetworkService _networkService;

    public TestOffline(
        ProductoService productoService,
        OfflineQueueService queueService,
        NetworkService networkService)
    {
        _productoService = productoService;
        _queueService = queueService;
        _networkService = networkService;
    }

    public async Task AgregarProductoOffline(string nombre, double precio)
    {
        // Forzar estado offline
        // (asume que NetworkService.IsOnline() puede ser falso para pruebas)
        var producto = new ProductoDto
        {
            Nombre = nombre,
            Precio = precio,
            SyncStatus = SyncStatus.PendingCreate
        };

        // Agregar a la cola
        await _queueService.EnqueueAsync(new OfflineOperation
        {
            OperationType = "Create",
            EntityType = "Producto",
            EntityData = System.Text.Json.JsonSerializer.Serialize(producto),
            CreatedAt = DateTime.UtcNow
        });

        // Guardar en cache local para simular UI
        var productos = await _productoService.GetProductosAsync();
        productos.Add(producto);
        await _productoService.GuardarProductosDtoEnCacheAsync(productos);

        Console.WriteLine($"Producto offline agregado: {nombre} - {precio}");
    }

    public async Task Sincronizar()
    {
        Console.WriteLine("Iniciando sincronización de cola...");
        await _queueService.ProcessQueueAsync();
        Console.WriteLine("Sincronización completada.");
    }
}