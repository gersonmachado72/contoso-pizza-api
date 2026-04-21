using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Models;
using ContosoPizza.Services;

namespace ContosoPizza.Controllers;

[Authorize] // 🔒 Apenas admin pode acessar
public class AdminAvaliacoesController : Controller
{
    private readonly AppDbContext _context;

    public AdminAvaliacoesController(AppDbContext context)
    {
        _context = context;
    }

    // Listar todas as avaliações
    public async Task<IActionResult> Index()
    {
        var avaliacoes = await _context.Avaliacoes
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync();
        return View(avaliacoes);
    }

    // Aprovar avaliação
    [HttpPost("Aprovar/{id}")]
    public async Task<IActionResult> Aprovar(int id)
    {
        var avaliacao = await _context.Avaliacoes.FindAsync(id);
        if (avaliacao == null)
            return NotFound();
        
        avaliacao.Aprovado = true;
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    // Excluir avaliação
    [HttpPost("Excluir/{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var avaliacao = await _context.Avaliacoes.FindAsync(id);
        if (avaliacao == null)
            return NotFound();
        
        _context.Avaliacoes.Remove(avaliacao);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    // Exportar relatório CSV
    [HttpGet("ExportarCSV")]
    public IActionResult ExportarCSV()
    {
        var avaliacoes = _context.Avaliacoes.ToList();
        var bytes = RelatorioAvaliacoesService.GerarRelatorioAvaliacoesCSV(avaliacoes);
        return File(bytes, "text/csv", $"avaliacoes_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }
}
