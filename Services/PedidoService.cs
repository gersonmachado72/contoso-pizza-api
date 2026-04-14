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
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            // 1. Salvar o pedido primeiro (sem os itens)
            var novoPedido = new Pedido
            {
                NomeCliente = pedido.NomeCliente,
                Endereco = pedido.Endereco,
                Telefone = pedido.Telefone,
                Observacao = pedido.Observacao,
                MetodoPagamento = pedido.MetodoPagamento,
                PagamentoConfirmado = pedido.PagamentoConfirmado,
                RestaurantId = pedido.RestaurantId,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                ValorTotal = pedido.ValorTotal,
                Itens = null // Sem itens por enquanto
            };
            
            _context.Pedidos.Add(novoPedido);
            _context.SaveChanges();
            
            // 2. Agora adicionar os itens com o PedidoId correto
            if (pedido.Itens != null && pedido.Itens.Any())
            {
                foreach (var item in pedido.Itens)
                {
                    var novoItem = new ItemPedido
                    {
                        PedidoId = novoPedido.Id,
                        Sabor = item.Sabor,
                        Tamanho = item.Tamanho,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario,
                        ObservacaoItem = item.ObservacaoItem
                    };
                    _context.ItensPedido.Add(novoItem);
                }
                _context.SaveChanges();
            }
            
            // 3. Atualizar o pedido original com o ID gerado
            pedido.Id = novoPedido.Id;
            
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception($"Erro ao salvar pedido: {ex.Message}", ex);
        }
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
