using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

public class HomeController : Controller
{
    private readonly PedidoService _pedidoService;
    private readonly EmailService _emailService;

    public HomeController(PedidoService pedidoService, EmailService emailService)
    {
        _pedidoService = pedidoService;
        _emailService = emailService;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        if (email == "admin@contosopizza.com" && password == "Admin@123")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("AdminPedidos");
        }
        
        ViewBag.Error = "E-mail ou senha inválidos!";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }

    public IActionResult AcessoNegado() => View();

    [Authorize]
    public IActionResult AdminPedidos()
    {
        var pedidos = _pedidoService.GetAll();
        return View(pedidos);
    }

    [Authorize]
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

    [HttpPost]
    public IActionResult FazerPedido([FromBody] PedidoViewModel pedidoVM)
    {
        if (pedidoVM == null || string.IsNullOrEmpty(pedidoVM.NomeCliente))
            return BadRequest("Dados do pedido inválidos");
        
        var pedido = new Pedido
        {
            NomeCliente = pedidoVM.NomeCliente,
            Endereco = pedidoVM.Endereco,
            Telefone = pedidoVM.Telefone,
            Email = pedidoVM.Email ?? "",
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
        
        // Enviar e-mail em background (não trava o fluxo)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.EnviarConfirmacaoPedido(pedidoVM.Email, pedidoVM.NomeCliente, pedido.Id, pedido.ValorTotal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no envio de e-mail: {ex.Message}");
            }
        });
        
        return View("PedidoConfirmado", pedido);
    }

    [Authorize]
    public IActionResult DownloadRelatorio()
    {
        var pedidos = _pedidoService.GetAll();
        var bytes = RelatorioService.GerarRelatorioVendasCSV(pedidos);
        return File(bytes, "text/csv", $"relatorio_pedidos_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }
}

public class PedidoViewModel
{
    public string? NomeCliente { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
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

    // Página pública para rastrear pedido (sem autenticação)
    [HttpGet]
    public IActionResult RastrearPedido(int? id, string? email)
    {
        if (id.HasValue && !string.IsNullOrEmpty(email))
        {
            var pedido = _pedidoService.Get(id.Value);
            if (pedido != null && pedido.Email == email)
            {
                return View(pedido);
            }
            ViewBag.Error = "Pedido não encontrado ou e-mail incorreto.";
        }
        return View();
    }
