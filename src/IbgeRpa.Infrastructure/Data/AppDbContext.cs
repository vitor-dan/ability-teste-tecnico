using IbgeRpa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IbgeRpa.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Indicador> Indicadores => Set<Indicador>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Indicador>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Nome).IsUnique();
            e.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            e.Property(x => x.Ultimo).IsRequired().HasMaxLength(100);
            e.Property(x => x.Anterior).IsRequired().HasMaxLength(100);
            e.Property(x => x.Variacao12Meses).HasMaxLength(100);
            e.Property(x => x.VariacaoNoAno).HasMaxLength(100);
            e.Property(x => x.Fonte).HasMaxLength(200);
            e.Property(x => x.Categoria).HasConversion<string>();
        });
    }
}