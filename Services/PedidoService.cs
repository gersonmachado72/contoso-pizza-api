using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class PedidoService
{
    private static AppDbContext? _context;

    public static void Initialize(AppDbContext context)
    {
        _context = context;
    }

    public static List<Pedido> GetAll()
    {
        if (_context == null) return new List<Pedido>();
        return _context.Pedidos
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataPedido)
            .ToList();
    }

    public static Pedido? Get(int id)
    {
        if (_context == null) return null;
        return _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefault(p => p.Id == id);
    }

    public static void Add(Pedido pedido)
    {
        if (_context == null) return;
        
        // Garantir que DataPedido está preenchida
        if (pedido.DataPedido == DateTime.MinValue)
        {
            pedido.DataPedido = DateTime.Now;
        }
        
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
        
        // Recarregar o pedido para obter o ID gerado
        if (pedido.Id == 0)
        {
            var saved = _context.Pedidos.OrderByDescending(p => p.Id).FirstOrDefault();
            if (saved != null) pedido.Id = saved.Id;
        }
    }

    public static void Update(Pedido pedido)
    {
        if (_context == null) return;
        _context.Pedidos.Update(pedido);
        _context.SaveChanges();
    }

    public static void Delete(int id)
    {
        if (_context == null) return;
        var pedido = Get(id);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
            _context.SaveChanges();
        }
    }
}
