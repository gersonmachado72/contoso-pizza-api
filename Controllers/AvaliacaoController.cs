using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Models;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvaliacaoController : ControllerBase
{
    private readonly AppDbContext _context;

    public AvaliacaoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarAvaliacao([FromBody] Avaliacao avaliacao)
    {
        Console.WriteLine($"=== Recebendo avaliação ===");
        Console.WriteLine($"PedidoId: {avaliacao?.PedidoId}");
        Console.WriteLine($"Nota: {avaliacao?.Nota}");
        
        if (avaliacao == null)
        {
            Console.WriteLine("Erro: avaliação é nula");
            return BadRequest(new { error = "Dados inválidos" });
        }
        
        if (avaliacao.Nota < 1 || avaliacao.Nota > 5)
        {
            Console.WriteLine($"Erro: nota inválida {avaliacao.Nota}");
            return BadRequest(new { error = "Nota deve ser entre 1 e 5" });
        }
        
        avaliacao.DataAvaliacao = DateTime.UtcNow;
        avaliacao.Aprovado = false;
        
        _context.Avaliacoes.Add(avaliacao);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"✅ Avaliação salva com sucesso!");
        return Ok(new { success = true, message = "Avaliação enviada com sucesso!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAvaliacoesAprovadas()
    {
        var avaliacoes = await _context.Avaliacoes
            .Where(a => a.Aprovado)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync();
        return Ok(avaliacoes);
    }
}
