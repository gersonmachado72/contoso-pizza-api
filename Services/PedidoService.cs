using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public class PedidoService
{
    private readonly AppDbContext _context;

    public PedidoService(AppDbContext context)
    {
        _context = context;
    }

    public List<Pedido> GetAll()
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataPedido)
            .ToList();
    }

    public Pedido? Get(int id)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Pedido pedido)
    {
        // Garantir que os itens estão corretamente vinculados
        if (pedido.Itens != null)
        {
            foreach (var item in pedido.Itens)
            {
                item.PedidoId = 0; // Será definido automaticamente pelo EF
                item.Id = 0;
            }
        }
        
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
        
        // Recarregar para garantir que os IDs estão corretos
        _context.Entry(pedido).State = EntityState.Detached;
    }

    public void Update(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var pedido = Get(id);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
            _context.SaveChanges();
        }
    }
}
