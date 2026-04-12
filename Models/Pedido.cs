using System;
using System.Collections.Generic;

namespace ContosoPizza.Models;

public class Pedido
{
    public int Id { get; set; }
    public string? NomeCliente { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataPedido { get; set; }
    public string? Status { get; set; }  // "Preparando", "Saiu para entrega", "Finalizado"
    public List<ItemPedido>? Itens { get; set; }
    public decimal ValorTotal { get; set; }
}

public class ItemPedido
{
    public int Id { get; set; }
    public string? Sabor { get; set; }
    public string? Tamanho { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
