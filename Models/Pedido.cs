using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Pedido
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome do cliente é obrigatório")]
    [MinLength(3)]
    public string? NomeCliente { get; set; }
    
    [Required]
    public string? Endereco { get; set; }
    
    [Required]
    [Phone]
    public string? Telefone { get; set; }
    
    public DateTime DataPedido { get; set; }
    public string? Status { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacao { get; set; }
    public string? EntregadorNome { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? MetodoPagamento { get; set; }
    public bool PagamentoConfirmado { get; set; }
    public int RestaurantId { get; set; } = 1;
    
    // Relacionamento correto
    public virtual ICollection<ItemPedido>? Itens { get; set; }
}
