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
    public IActionResult CriarPedido([FromBody] PedidoViewModel request)
    {
        if (request == null || string.IsNullOrEmpty(request.NomeCliente))
        {
            return BadRequest(new { error = "Nome do cliente é obrigatório" });
        }

        // Calcular preços
        decimal total = 0;
        var itens = new List<ItemPedido>();

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

                    itens.Add(new ItemPedido
                    {
                        Sabor = item.Sabor,
                        Tamanho = item.Tamanho,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = preco
                    });
                    total += preco * item.Quantidade;
                }
            }
        }

        var pedido = new Pedido
        {
            NomeCliente = request.NomeCliente,
            Endereco = request.Endereco ?? "",
            Telefone = request.Telefone ?? "",
            Observacao = request.Observacao ?? "",
            MetodoPagamento = request.MetodoPagamento ?? "Dinheiro",
            DataPedido = DateTime.UtcNow,
            Status = "Preparando",
            PagamentoConfirmado = false,
            RestaurantId = 1,
            Itens = itens,
            ValorTotal = total
        };

        _pedidoService.Add(pedido);

        return Ok(new { success = true, pedidoId = pedido.Id, message = "Pedido criado com sucesso" });
    }
}
