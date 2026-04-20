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
            Console.WriteLine("❌ SendGrid: API Key não configurada!");
            return false;
        }

        var fromEmail = "todas.pizza@gmail.com";
        var fromName = "Todas Pizzarias";

        // 🔥 LINK CORRETO - Página pública de rastreamento
        var linkRastreio = $"https://contoso-pizza-api.onrender.com/Home/RastrearPedido?id={pedidoId}&email={Uri.EscapeDataString(email)}";

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
                from = new { email = fromEmail, name = fromName },
                reply_to = new { email = fromEmail, name = fromName },
                content = new[]
                {
                    new
                    {
                        type = "text/html",
                        value = $@"
                            <h1>🍕 Pedido Confirmado!</h1>
                            <p>Olá {nomeCliente}, seu pedido <strong>#{pedidoId}</strong> foi recebido com sucesso!</p>
                            <p><strong>Total:</strong> R$ {total:F2}</p>
                            <p><strong>Status atual:</strong> Preparando 🔧</p>
                            <p>Acompanhe seu pedido em tempo real:</p>
                            <p><a href='{linkRastreio}' style='background:#e67e22; color:white; padding:10px 20px; text-decoration:none; border-radius:25px; display:inline-block;'>📦 Acompanhar Pedido</a></p>
                            <hr>
                            <p style='font-size:12px; color:#666;'>Este é um e-mail automático. Por favor, não responda.</p>
                        "
                    }
                }
            };
            
            var json = JsonSerializer.Serialize(content);
            var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", 
                new StringContent(json, Encoding.UTF8, "application/json"));
            
            var responseBody = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ E-mail enviado com sucesso para {email}");
                return true;
            }
            
            Console.WriteLine($"❌ SendGrid erro {response.StatusCode}: {responseBody}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exceção ao enviar e-mail: {ex.Message}");
            return false;
        }
    }
}
