using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AutService.JwAuthLogin.Application.Services
{
    public static class EmailService
    {
        /// <summary>
        /// Envía un correo electrónico utilizando SendGrid.
        /// </summary>
        /// <param name="toEmail">Dirección de correo del destinatario.</param>
        /// <param name="subject">Asunto del correo.</param>
        /// <param name="plainTextContent">Contenido del correo en texto plano.</param>
        /// <param name="htmlContent">Contenido del correo en formato HTML.</param>
        /// <param name="sendGridApiKey">API Key de SendGrid.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public static async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent, string sendGridApiKey)
        {
            if (string.IsNullOrWhiteSpace(sendGridApiKey))
            {
                throw new InvalidOperationException("La clave API de SendGrid no está configurada.");
            }

            var client = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress("no-reply@yourapp.com", "Your App");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Error al enviar el correo: {response.StatusCode}");
            }
        }
    }
}
