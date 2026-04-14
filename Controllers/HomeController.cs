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

    [HttpPost("FazerPedido")]  // 👈 ROTA EXPLÍCITA
    public IActionResult FazerPedido([FromBody] PedidoRequest request)
    {
        try
        {
            _logger.LogInformation("=== INICIANDO PROCESSAMENTO DO PEDIDO ===");
            _logger.LogInformation($"Request recebido: Nome={request?.NomeCliente}");

            if (request == null)
                return BadRequest(new { error = "Dados do pedido não enviados" });

            if (string.IsNullOrWhiteSpace(request.NomeCliente))
                return BadRequest(new { error = "Nome do cliente é obrigatório" });

            if (request.Itens == null || !request.Itens.Any())
                return BadRequest(new { error = "Adicione pelo menos uma pizza ao pedido" });

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
            var pizzas = _pizzaService.GetAll();
            
            foreach (var item in request.Itens)
            {
                var pizza = pizzas.FirstOrDefault(p => p.Name == item.Sabor);
                if (pizza == null) continue;
                
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
            
            pedido.ValorTotal = total;
            _pedidoService.Add(pedido);
            _logger.LogInformation($"Pedido #{pedido.Id} salvo com sucesso!");
            
            return View("PedidoConfirmado", pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pedido");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    public IActionResult AdminPedidos()
    {
        var pedidos = _pedidoService.GetAll();
        return View(pedidos);
    }
    
    [HttpPost("AtualizarStatus")]
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
