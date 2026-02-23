namespace MiProyecto.Domain;

public class Producto
{
    public int Id_producto { get; set; }
    public required string Nombre { get; set; }
    public required double Precio { get; set; }
}
