using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class Pedido
{
    public int Id { get; set; }
    
    [Required]
    public string? NomeCliente { get; set; }
    
    [Required]
    public string? Endereco { get; set; }
    
    [Required]
    [Phone]
    public string? Telefone { get; set; }
    
    public DateTime DataPedido { get; set; }
    public string? Status { get; set; }
    public List<ItemPedido>? Itens { get; set; }
    public decimal ValorTotal { get; set; }
    
    public string? Observacao { get; set; }
    public string? EntregadorNome { get; set; }
    public DateTime? DataEntrega { get; set; }
    public string? MetodoPagamento { get; set; }
    public bool PagamentoConfirmado { get; set; }
    public int RestaurantId { get; set; } = 1;
}

public class ItemPedido
{
    public int Id { get; set; }
    public string? Sabor { get; set; }
    public string? Tamanho { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;
    public string? ObservacaoItem { get; set; }
}