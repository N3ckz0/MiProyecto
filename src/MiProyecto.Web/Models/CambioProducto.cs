public class CambioProducto
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public string Operacion { get; set; }

    public string DatosJson { get; set; }

    public DateTime Fecha { get; set; }

    public bool Sincronizado { get; set; }
}