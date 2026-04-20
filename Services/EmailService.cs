using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ContosoPizza.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> EnviarConfirmacaoPedido(string email, string nomeCliente, int pedidoId, decimal total)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine($"⚠️ SendGrid não configurado. E-mail não enviado para {email}");
            return false;
        }

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            
            var content = new
            {
                personalizations = new[]
                {
                    new
                    {
                        to = new[] { new { email = email } },
                        subject = $"Pedido #{pedidoId} confirmado!"
                    }
                },
                from = new { email = "todas.pizza@gmail.com", name = "ContosoPizza" },
                content = new[]
                {
                    new
                    {
                        type = "text/html",
                        value = $@"
                            <h1>🍕 Pedido Confirmado!</h1>
                            <p>Olá {nomeCliente}, seu pedido <strong>#{pedidoId}</strong> foi recebido!</p>
                            <p><strong>Total:</strong> R$ {total:F2}</p>
                            <p>Acesse: <a href='https://contoso-pizza-api.onrender.com/Home/AdminPedidos'>Acompanhar pedido</a></p>
                        "
                    }
                }
            };
            
            var json = JsonSerializer.Serialize(content);
            var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", 
                new StringContent(json, Encoding.UTF8, "application/json"));
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ E-mail enviado para {email}");
                return true;
            }
            
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro SendGrid: {error}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar e-mail: {ex.Message}");
            return false;
        }
    }
}
