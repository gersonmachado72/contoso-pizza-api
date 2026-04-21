using System.Text;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class RelatorioAvaliacoesService
{
    public static byte[] GerarRelatorioAvaliacoesCSV(List<Avaliacao> avaliacoes)
    {
        var csv = new StringBuilder();
        csv.AppendLine("ID do Pedido;Cliente;Nota;Comentário;Data;Aprovado");
        
        foreach (var av in avaliacoes)
        {
            csv.AppendLine($"{av.PedidoId};{av.ClienteNome};{av.Nota};{av.Comentario ?? ""};{av.DataAvaliacao:dd/MM/yyyy HH:mm};{(av.Aprovado ? "Sim" : "Não")}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}
