using System.Text;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class RelatorioService
{
    public static byte[] GerarRelatorioVendasCSV(List<Pedido> pedidos)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Pedido ID;Cliente;Data;Valor Total;Status;Itens");
        
        foreach (var pedido in pedidos)
        {
            var itens = pedido.Itens != null 
                ? string.Join("; ", pedido.Itens.Select(i => $"{i.Quantidade}x {i.Sabor}"))
                : "";
                
            csv.AppendLine($"{pedido.Id};{pedido.NomeCliente};{pedido.DataPedido:dd/MM/yyyy HH:mm};{pedido.ValorTotal:F2};{pedido.Status};{itens}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}
