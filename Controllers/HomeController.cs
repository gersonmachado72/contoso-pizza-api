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

    public IActionResult AdminPedidos()
    {
        var pedidos = _pedidoService.GetAll();
        return View(pedidos);
    }

    public IActionResult PedidoConfirmado(int id)
    {
        var pedido = _pedidoService.Get(id);
        if (pedido == null)
        {
            return NotFound();
        }
        return View(pedido);
    }
}
