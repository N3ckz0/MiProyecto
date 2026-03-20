public enum SyncStatus
{
    Synced,
    PendingCreate,
    PendingUpdate,
    PendingDelete
}

public class ProductoDto
{
    public int Id_producto { get; set; }
    public string Nombre { get; set; } = "";
    public double Precio { get; set; }
    public SyncStatus SyncStatus { get; set; }
}