using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [HttpPost("FazerPedido")]  // Rota explícita
    public IActionResult FazerPedido([FromBody] PedidoRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.NomeCliente))
            return BadRequest("Nome do cliente é obrigatório");

        var pedido = new Pedido
        {
            NomeCliente = request.NomeCliente,
            Endereco = request.Endereco ?? "",
            Telefone = request.Telefone ?? "",
            Observacao = request.Observacao ?? "",
            MetodoPagamento = request.MetodoPagamento ?? "Dinheiro",
            PagamentoConfirmado = false,
            RestaurantId = 1,
            DataPedido = DateTime.Now,
            Status = "Preparando",
            Itens = new List<ItemPedido>(),
            ValorTotal = 0
        };
        
        decimal total = 0;
        var pizzas = PizzaService.GetAll();
        
        foreach (var item in request.Itens ?? new List<ItemPedidoRequest>())
        {
            var pizza = pizzas.FirstOrDefault(p => p.Name == item.Sabor);
            if (pizza != null)
            {
                decimal preco = pizza.Price;
                if (item.Tamanho == "Média") preco += 5;
                else if (item.Tamanho == "Grande") preco += 10;
                
                pedido.Itens.Add(new ItemPedido
                {
                    Sabor = item.Sabor,
                    Tamanho = item.Tamanho,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = preco
                });
                total += preco * item.Quantidade;
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
    
    [HttpPost("AtualizarStatus")]
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
