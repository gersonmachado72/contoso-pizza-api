using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Pizza>> GetAll()
    {
        return PizzaService.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<Pizza> Get(int id)
    {
        var pizza = PizzaService.Get(id);
        if (pizza == null)
        {
            return NotFound();
        }
        return Ok(pizza);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Pizza pizza)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        PizzaService.Add(pizza);
        return CreatedAtAction(nameof(Get), new { id = pizza.Id }, pizza);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Pizza pizza)
    {
        if (id != pizza.Id)
        {
            return BadRequest();
        }
        
        var existing = PizzaService.Get(id);
        if (existing == null)
        {
            return NotFound();
        }
        
        PizzaService.Update(pizza);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var pizza = PizzaService.Get(id);
        if (pizza == null)
        {
            return NotFound();
        }
        
        PizzaService.Delete(id);
        return NoContent();
    }
}
