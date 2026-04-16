using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TesteController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "API está funcionando!", timestamp = DateTime.UtcNow });
    }

    [HttpPost]
    public IActionResult Post([FromBody] object dados)
    {
        Console.WriteLine($"POST recebido: {dados}");
        return Ok(new { success = true, recebido = dados });
    }
}
