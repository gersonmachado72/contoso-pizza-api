using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoPizza.Models;

public class ItemPedido
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "PedidoId é obrigatório")]
    public int PedidoId { get; set; }  // Chave estrangeira - OBRIGATÓRIA
    
    [ForeignKey("PedidoId")]
    public virtual Pedido? Pedido { get; set; }
    
    public string? Sabor { get; set; }
    public string? Tamanho { get; set; }
    
    [Range(1, 99)]
    public int Quantidade { get; set; }
    
    [Range(0.01, 999.99)]
    public decimal PrecoUnitario { get; set; }
    
    public string? ObservacaoItem { get; set; }
    
    [NotMapped]
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
