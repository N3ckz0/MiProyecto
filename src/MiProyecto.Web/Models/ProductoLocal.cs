using System.Text.Json.Serialization;

namespace MiProyecto.Web.Models;

public class ProductoLocal
{
    [JsonPropertyName("Id_producto")]
    public int Id_producto { get; set; }

    [JsonPropertyName("Nombre")]
    public string? Nombre { get; set; }

    [JsonPropertyName("Precio")]
    public double? Precio { get; set; }

    [JsonPropertyName("IsSynced")]
    public bool IsSynced { get; set; }

    [JsonPropertyName("LastModified")]
    public DateTime LastModified { get; set; }
}