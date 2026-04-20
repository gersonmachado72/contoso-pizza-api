using SendGrid;
using SendGrid.Helpers.Mail;
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
        if (string.IsNullOrEmpty(apiKey)) return false;

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("contato@contosopizza.com", "ContosoPizza");
        var to = new EmailAddress(email, nomeCliente);
        var subject = $"Pedido #{pedidoId} confirmado!";
        
        var htmlContent = $@"
            <h1>🍕 Pedido Confirmado!</h1>
            <p>Olá {nomeCliente}, seu pedido <strong>#{pedidoId}</strong> foi recebido e está sendo preparado.</p>
            <p><strong>Total:</strong> R$ {total:F2}</p>
            <p>Acompanhe pelo site: <a href='https://contoso-pizza-api.onrender.com/Home/AdminPedidos'>Clique aqui</a></p>
            <p>Obrigado por escolher a ContosoPizza!</p>
        ";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
        var response = await client.SendEmailAsync(msg);
        
        return response.IsSuccessStatusCode;
    }
}
