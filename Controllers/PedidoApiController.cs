using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/[controller]")]  // Rota: /api/pedidoapi
public class PedidoApiController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public PedidoApiController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public IActionResult CriarPedido([FromBody] PedidoViewModel pedidoVM)
    {
        if (pedidoVM == null || string.IsNullOrEmpty(pedidoVM.NomeCliente))
        {
            return BadRequest(new { error = "Dados do pedido inválidos" });
        }

        var pedido = new Pedido
        {
            NomeCliente = pedidoVM.NomeCliente,
            Endereco = pedidoVM.Endereco ?? "",
            Telefone = pedidoVM.Telefone ?? "",
            Observacao = pedidoVM.Observacao ?? "",
            MetodoPagamento = pedidoVM.MetodoPagamento ?? "Dinheiro",
            DataPedido = DateTime.UtcNow,
            Status = "Preparando",
            PagamentoConfirmado = false,
            RestaurantId = 1,
            Itens = new List<ItemPedido>(),
            ValorTotal = 0
        };

        decimal total = 0;

        if (pedidoVM.Itens != null)
        {
            foreach (var item in pedidoVM.Itens)
            {
                decimal precoBase = 0;
                var pizza = PizzaService.GetAll().FirstOrDefault(p => p.Name == item.Sabor);
                if (pizza != null)
                {
                    precoBase = pizza.Price;
                    if (item.Tamanho == "Média") precoBase += 5;
                    else if (item.Tamanho == "Grande") precoBase += 10;
                }

                pedido.Itens.Add(new ItemPedido
                {
                    Sabor = item.Sabor,
                    Tamanho = item.Tamanho,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = precoBase
                });
                total += precoBase * item.Quantidade;
            }
        }

        pedido.ValorTotal = total;
        _pedidoService.Add(pedido);

        return Ok(new { 
            success = true, 
            pedidoId = pedido.Id,
            redirectUrl = Url.Action("PedidoConfirmado", "Home", new { id = pedido.Id })
        });
    }
}

public class PedidoViewModel
{
    public string? NomeCliente { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Observacao { get; set; }
    public string? MetodoPagamento { get; set; }
    public List<ItemPedidoVM>? Itens { get; set; }
}

public class ItemPedidoVM
{
    public string? Sabor { get; set; }
    public string? Tamanho { get; set; }
    public int Quantidade { get; set; }
}
