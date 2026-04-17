using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;
    private readonly PedidoService _pedidoService;

    public HomeController(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        PedidoService pedidoService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _pedidoService = pedidoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("AdminPedidos");
            }
        }
        
        ViewBag.Error = "E-mail ou senha inválidos!";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index");
    }

    public IActionResult AcessoNegado()
    {
        return View();
    }

    [Authorize] // 🔒 Protegido - só acessível com login
    public IActionResult AdminPedidos()
    {
        var pedidos = _pedidoService.GetAll();
        return View(pedidos);
    }

    [HttpPost]
    [Authorize] // 🔒 Protegido
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

    [HttpPost]
    public IActionResult FazerPedido([FromBody] PedidoViewModel pedidoVM)
    {
        // Este método continua público (clientes podem fazer pedidos sem login)
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
