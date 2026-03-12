namespace MiProyecto.Domain;

public class Producto
{
    public int Id_producto { get; set; }
    public string? Nombre { get; set; }
    public double? Precio { get; set; }
    public bool IsOffline { get; set; } = false;
}
