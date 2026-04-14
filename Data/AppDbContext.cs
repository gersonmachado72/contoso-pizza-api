using Microsoft.EntityFrameworkCore;
using ContosoPizza.Models;

namespace ContosoPizza.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }
    public DbSet<Avaliacao> Avaliacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurar relacionamento Pedido -> ItensPedido
        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne(i => i.Pedido)
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configurar tabela ItemPedido
        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.ToTable("ItemPedido");
            entity.Property(e => e.PedidoId).IsRequired(); // Forçar NOT NULL
            entity.HasOne(i => i.Pedido)
                  .WithMany(p => p.Itens)
                  .HasForeignKey(i => i.PedidoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
