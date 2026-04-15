using System;
using System.Collections.Generic;
using System.Linq;
using ContosoPizza.Models;
using ContosoPizza.Data;
using Microsoft.EntityFrameworkCore;

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
        // Garantir que DataPedido está em UTC
        if (pedido.DataPedido.Kind != DateTimeKind.Utc)
        {
            pedido.DataPedido = DateTime.SpecifyKind(pedido.DataPedido, DateTimeKind.Utc);
        }
        
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
        Console.WriteLine($"✅ Pedido #{pedido.Id} salvo com total R$ {pedido.ValorTotal}");
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
