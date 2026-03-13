namespace MiProyecto.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public required string Image { get; set; }
    public required string Nombre { get; set; }
    public required string Rfc { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
