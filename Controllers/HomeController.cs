using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
    private readonly PedidoService _pedidoService;

    public HomeController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult FazerPedido([FromBody] PedidoViewModel pedidoVM)
    {
        if (pedidoVM == null || string.IsNullOrEmpty(pedidoVM.NomeCliente))
        {
            return BadRequest("Dados do pedido inválidos");
        }
        
        var pedido = new Pedido
        {
            NomeCliente = pedidoVM.NomeCliente,
            Endereco = pedidoVM.Endereco,
            Telefone = pedidoVM.Telefone,
            Observacao = pedidoVM.Observacao ?? "",
            MetodoPagamento = pedidoVM.MetodoPagamento ?? "Dinheiro",
            DataPedido = DateTime.Now,
            Status = "Preparando",
            PagamentoConfirmado = false,
            RestaurantId = 1,
            Itens = new List<ItemPedido>()
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
        
        return View("PedidoConfirmado", pedido);
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
