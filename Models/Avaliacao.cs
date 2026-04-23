using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Avaliacao
{
    public int Id { get; set; }
    
    [Required]
    public int PedidoId { get; set; }
    
    [Range(1, 5)]
    public int Nota { get; set; }
    
    [StringLength(500)]
    public string? Comentario { get; set; }
    
    public string? ClienteNome { get; set; }
    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;
    public bool Aprovado { get; set; } = false;
}
