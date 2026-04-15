using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/[controller]")]
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
            redirectUrl = $"/Home/PedidoConfirmado?id={pedido.Id}"
        });
    }
}
