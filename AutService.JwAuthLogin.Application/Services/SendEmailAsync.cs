using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _apiKey;

    public EmailService(IConfiguration config)
    {
        // tener configurado en appsettings.json o variables de entorno:
        // "SendGrid": { "ApiKey": "TU_API_KEY_AQUI" }
        _apiKey = config["SendGrid:ApiKey"]
            ?? throw new ArgumentNullException("No se encontró la API Key de SendGrid");
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string plainTextContent,
        string htmlContent)
    {
        var client = new SendGridClient(_apiKey);

        var from = new EmailAddress("matias.urquieta@devalpo.cl", "InstaScoreApp");
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        // Este es el campo "Responder a" que verá el usuario en su cliente de correo
        msg.ReplyTo = new EmailAddress("matias.urquieta@devalpo.cl", "Soporte");

        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new Exception($"Error al enviar el correo: {response.StatusCode} - {body}");
        }
    }
}
