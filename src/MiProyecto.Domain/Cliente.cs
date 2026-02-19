namespace MiProyecto.Domain;

public class Cliente
{
    public int Id { get; set; }
    public required string image { get; set; }
    public required string Nombre { get; set; }
    public required string Rfc { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
