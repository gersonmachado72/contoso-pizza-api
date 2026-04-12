using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
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
        
        // Converter ViewModel para Model
        var pedido = new Pedido
        {
            NomeCliente = pedidoVM.NomeCliente,
            Endereco = pedidoVM.Endereco,
            Telefone = pedidoVM.Telefone,
            Itens = new List<ItemPedido>()
        };
        
        if (pedidoVM.Itens != null)
        {
            foreach (var item in pedidoVM.Itens)
            {
                // Calcular preço baseado no tamanho (simplificado)
                decimal precoBase = 0;
                var pizza = PizzaService.GetAll().FirstOrDefault(p => p.Name == item.Sabor);
                if (pizza != null)
                {
                    precoBase = pizza.Price;
                    
                    // Ajuste por tamanho
                    if (item.Tamanho == "Média") precoBase += 5;
                    else if (item.Tamanho == "Grande") precoBase += 10;
                    else if (item.Tamanho == "Família") precoBase += 15;
                }
                
                pedido.Itens.Add(new ItemPedido
                {
                    Sabor = item.Sabor,
                    Tamanho = item.Tamanho,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = precoBase
                });
            }
        }
        
        PedidoService.Add(pedido);
        
        return View("PedidoConfirmado", pedido);
    }
    
    public IActionResult AdminPedidos()
    {
        var pedidos = PedidoService.GetAll();
        return View(pedidos);
    }
    
    [HttpPost]
    public IActionResult AtualizarStatus(int id, string status)
    {
        PedidoService.UpdateStatus(id, status);
        return RedirectToAction("AdminPedidos");
    }
}

// ViewModel para receber dados do formulário
public class PedidoViewModel
{
    public string? NomeCliente { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public List<ItemPedidoVM>? Itens { get; set; }
}

public class ItemPedidoVM
{
    public string? Sabor { get; set; }
    public string? Tamanho { get; set; }
    public int Quantidade { get; set; }
}
