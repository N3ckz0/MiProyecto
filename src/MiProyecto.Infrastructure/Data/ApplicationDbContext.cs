using Microsoft.EntityFrameworkCore;
using MiProyecto.Domain.Entities;

namespace MiProyecto.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
}
