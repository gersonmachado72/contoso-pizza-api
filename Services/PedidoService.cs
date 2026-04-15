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
        Console.WriteLine("✅ PedidoService inicializado");
    }

    public List<Pedido> GetAll()
    {
        Console.WriteLine("📋 Buscando todos os pedidos");
        return _context.Pedidos
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataPedido)
            .ToList();
    }

    public Pedido? Get(int id)
    {
        Console.WriteLine($"🔍 Buscando pedido {id}");
        return _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Pedido pedido)
    {
        Console.WriteLine($"💾 Salvando pedido para {pedido.NomeCliente}, total: {pedido.ValorTotal}");
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
        Console.WriteLine($"✅ Pedido {pedido.Id} salvo com sucesso!");
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
