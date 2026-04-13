using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/[controller]")]          // 👈 ROTA CORRETA: /api/pizza
public class PizzaController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Pizza>> GetAll()
    {
        var pizzas = PizzaService.GetAll();
        if (pizzas == null || pizzas.Count == 0)
        {
            return Ok(new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Margherita", Price = 9.99M, IsVegetarian = true },
                new Pizza { Id = 2, Name = "Pepperoni", Price = 12.99M, IsVegetarian = false }
            });
        }
        return Ok(pizzas);
    }

    [HttpGet("{id}")]
    public ActionResult<Pizza> Get(int id)
    {
        var pizza = PizzaService.Get(id);
        if (pizza == null) return NotFound();
        return Ok(pizza);
    }
}
