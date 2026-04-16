using System;
using System.Collections.Generic;
using System.Linq;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public class PedidoService
{
    private static List<Pedido> _pedidos = new();
    private static int _nextId = 1;

    public List<Pedido> GetAll()
    {
        return _pedidos.OrderByDescending(p => p.DataPedido).ToList();
    }

    public Pedido? Get(int id)
    {
        return _pedidos.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Pedido pedido)
    {
        pedido.Id = _nextId++;
        pedido.DataPedido = DateTime.Now;
        pedido.Status = "Preparando";
        
        if (pedido.Itens != null)
        {
            pedido.ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        }
        
        _pedidos.Add(pedido);
        Console.WriteLine($"✅ Pedido #{pedido.Id} salvo em memória");
    }

    public void Update(Pedido pedido)
    {
        var index = _pedidos.FindIndex(p => p.Id == pedido.Id);
        if (index != -1) _pedidos[index] = pedido;
    }

    public void Delete(int id)
    {
        var pedido = Get(id);
        if (pedido != null) _pedidos.Remove(pedido);
    }
}
