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
        Console.WriteLine($"📋 Carregando {pizzas.Count} pizzas para o Admin");
        return View(pizzas);
    }

    [HttpPost("UploadFoto/{id}")]
    public async Task<IActionResult> UploadFoto(int id, IFormFile foto)
    {
        Console.WriteLine($"📸 Upload iniciado para pizza ID: {id}");
        Console.WriteLine($"📁 Arquivo: {foto?.FileName}, Tamanho: {foto?.Length} bytes");
        
        var pizza = PizzaService.Get(id);
        if (pizza == null)
        {
            Console.WriteLine($"❌ Pizza ID {id} não encontrada!");
            return NotFound();
        }

        if (foto == null || foto.Length == 0)
        {
            Console.WriteLine("❌ Nenhum arquivo enviado!");
            TempData["Error"] = "Nenhum arquivo selecionado.";
            return RedirectToAction("Index");
        }

        try
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(foto, "pizzas");
            Console.WriteLine($"☁️ Resposta do Cloudinary: {(string.IsNullOrEmpty(imageUrl) ? "FALHOU" : imageUrl)}");
            
            if (!string.IsNullOrEmpty(imageUrl))
            {
                pizza.ImageUrl = imageUrl;
                PizzaService.Update(pizza);
                Console.WriteLine($"✅ Foto salva para pizza {pizza.Name}: {imageUrl}");
                TempData["Success"] = "Foto atualizada com sucesso!";
            }
            else
            {
                Console.WriteLine("❌ Cloudinary retornou URL vazia!");
                TempData["Error"] = "Erro ao fazer upload da foto para o Cloudinary.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ EXCEÇÃO: {ex.Message}");
            TempData["Error"] = $"Erro: {ex.Message}";
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
            Console.WriteLine($"🗑️ Foto removida da pizza {pizza.Name}");
        }
        return RedirectToAction("Index");
    }
}
