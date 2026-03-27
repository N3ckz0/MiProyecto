public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "User";
    public string? Theme { get; set; }
    public string? DbName { get; set; }
    public bool IsConfigured { get; set; } = false;
}