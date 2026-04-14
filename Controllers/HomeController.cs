using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.Extensions.Logging;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
    private readonly PizzaService _pizzaService;
    private readonly PedidoService _pedidoService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(PizzaService pizzaService, PedidoService pedidoService, ILogger<HomeController> logger)
    {
        _pizzaService = pizzaService;
        _pedidoService = pedidoService;
        _logger = logger;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult FazerPedido([FromBody] PedidoRequest request)
    {
        try
        {
            _logger.LogInformation("Recebendo pedido: {@request}", request);

            if (request == null)
            {
                _logger.LogWarning("Request é nulo");
                return BadRequest("Dados do pedido não enviados");
            }

            if (string.IsNullOrEmpty(request.NomeCliente))
            {
                _logger.LogWarning("Nome do cliente vazio");
                return BadRequest("Nome do cliente é obrigatório");
            }

            var pedido = new Pedido
            {
                NomeCliente = request.NomeCliente,
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
                    var pizza = _pizzaService.GetAll().FirstOrDefault(p => p.Name == item.Sabor);
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
            
            _pedidoService.Add(pedido);
            _logger.LogInformation("Pedido {PedidoId} salvo com sucesso", pedido.Id);
            
            return View("PedidoConfirmado", pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pedido");
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
    
    public IActionResult AdminPedidos()
    {
        var pedidos = _pedidoService.GetAll();
        return View(pedidos);
    }
    
    [HttpPost]
    public IActionResult AtualizarStatus(int id, string status, string entregador, bool pagamentoConfirmado)
    {
        var pedido = _pedidoService.Get(id);
        if (pedido != null)
        {
            pedido.Status = status;
            pedido.EntregadorNome = entregador;
            pedido.PagamentoConfirmado = pagamentoConfirmado;
            if (status == "Finalizado") pedido.DataEntrega = DateTime.Now;
            _pedidoService.Update(pedido);
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
