using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult FazerPedido([FromBody] PedidoRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.NomeCliente))
        {
            return BadRequest("Dados inválidos");
        }
        
        var pedido = new Pedido
        {
            NomeCliente = request.NomeCliente ?? string.Empty,
            Endereco = request.Endereco ?? string.Empty,
            Telefone = request.Telefone ?? string.Empty,
            Observacao = request.Observacao ?? string.Empty,
            MetodoPagamento = request.MetodoPagamento ?? "Dinheiro",
            PagamentoConfirmado = false,
            RestaurantId = 1,
            DataPedido = DateTime.Now,
            Status = "Preparando",
            Itens = new List<ItemPedido>(),
            ValorTotal = 0
        };
        
        decimal total = 0;
        if (request.Itens != null)
        {
            foreach (var item in request.Itens)
            {
                var pizza = PizzaService.GetAll().FirstOrDefault(p => p.Name == item.Sabor);
                if (pizza != null)
                {
                    decimal preco = pizza.Price;
                    if (item.Tamanho == "Média") preco += 5;
                    else if (item.Tamanho == "Grande") preco += 10;
                    
                    var itemPedido = new ItemPedido
                    {
                        Sabor = item.Sabor,
                        Tamanho = item.Tamanho,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = preco
                    };
                    pedido.Itens.Add(itemPedido);
                    total += preco * item.Quantidade;
                }
            }
        }
        pedido.ValorTotal = total;
        
        PedidoService.Add(pedido);
        return View("PedidoConfirmado", pedido);
    }
    
    public IActionResult AdminPedidos()
    {
        var pedidos = PedidoService.GetAll();
        return View(pedidos);
    }
    
    [HttpPost]
    public IActionResult AtualizarStatus(int id, string status, string entregador, bool pagamentoConfirmado)
    {
        var pedido = PedidoService.Get(id);
        if (pedido != null)
        {
            pedido.Status = status;
            pedido.EntregadorNome = entregador;
            pedido.PagamentoConfirmado = pagamentoConfirmado;
            if (status == "Finalizado") pedido.DataEntrega = DateTime.Now;
            PedidoService.Update(pedido);
        }
        return RedirectToAction("AdminPedidos");
    }
}

public class PedidoRequest
{
    public string? NomeCliente { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Observacao { get; set; }
    public string? MetodoPagamento { get; set; }
    public List<ItemPedidoRequest>? Itens { get; set; }
}

public class ItemPedidoRequest
{
    public string? Sabor { get; set; }
    public string? Tamanho { get; set; }
    public int Quantidade { get; set; }
}
