using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

[Authorize]
[Route("AdminPizzas")]
public class AdminPizzasController : Controller
{
    private readonly CloudinaryService _cloudinaryService;

    public AdminPizzasController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    public IActionResult Index()
    {
        var pizzas = PizzaService.GetAll();
        return View(pizzas);
    }

    [HttpPost("UploadFoto/{id}")]
    public async Task<IActionResult> UploadFoto(int id, IFormFile foto)
    {
        var pizza = PizzaService.Get(id);
        if (pizza == null)
            return NotFound();

        if (foto != null && foto.Length > 0)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(foto, "pizzas");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                pizza.ImageUrl = imageUrl;
                PizzaService.Update(pizza);
                TempData["Success"] = "Foto atualizada com sucesso!";
            }
            else
            {
                TempData["Error"] = "Erro ao fazer upload da foto.";
            }
        }
        
        return RedirectToAction("Index");
    }

    [HttpPost("RemoverFoto/{id}")]
    public IActionResult RemoverFoto(int id)
    {
        var pizza = PizzaService.Get(id);
        if (pizza != null)
        {
            pizza.ImageUrl = null;
            PizzaService.Update(pizza);
            TempData["Success"] = "Foto removida com sucesso!";
        }
        return RedirectToAction("Index");
    }
}
