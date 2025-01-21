using data_aparta_.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Utils
{
    public  class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpServer"];
            _smtpPort = int.Parse(configuration["Email:SmtpPort"]);
            _fromEmail = configuration["Email:FromEmail"];
            _password = configuration["Email:Password"];
        }

        public async Task SendHtmlEmailAsync(string to, string subject, string htmlContent)
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(_fromEmail, _password)
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
        public async Task EnviarFacturasAsync(string emailDestinatario, List<Factura> facturas)
        {
            if (string.IsNullOrWhiteSpace(emailDestinatario) || facturas == null || facturas.Count == 0)
                throw new ArgumentException("El email del destinatario o la lista de facturas no es válida.");

            // Generar el contenido del email
            var htmlContent = GenerarHtmlFacturas(facturas);

            // Enviar el email con todas las facturas
            await SendHtmlEmailAsync(
                to: emailDestinatario,
                subject: $"Resumen de Facturas - {DateTime.Now:dd/MM/yyyy}",
                htmlContent: htmlContent
            );
        }

        private string GenerarHtmlFacturas(List<Factura> facturas)
        {
            if (facturas == null || facturas.Count == 0)
                throw new ArgumentException("No hay facturas disponibles para generar el correo.");

            // Obtener los datos del inquilino de la primera factura
            var inquilino = facturas.FirstOrDefault()?.Inmueble?.Contrato?.Inquilino;

            var nombreInquilino = inquilino?.Inquilinonombre ?? "N/A";
            var correoInquilino = inquilino?.Inquilinocorreo ?? "N/A";

            // Construir las filas dinámicamente
            var rowsBuilder = new StringBuilder();
            foreach (var factura in facturas)
            {
                rowsBuilder.Append($@"
            <tr>
                <td>{factura.Inmueble?.Codigo ?? "N/A"}</td>
                <td>{factura.Fechapago?.ToString("dd/MM/yyyy") ?? "Sin Fecha"}</td>
                <td>{factura.Estado ?? "Desconocido"}</td>
                <td>{factura.Monto?.ToString("C2") ?? "N/A"}</td>
            </tr>");
            }

            // Construir el HTML completo
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ font-family: Poppins, sans-serif; background-color: #f9f9f9; }}
        .invoice-container {{ max-width: 600px; margin: 20px auto; background: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
        .invoice-header {{ text-align: center; border-bottom: 2px solid #094152; margin-bottom: 20px; padding-bottom: 10px; }}
        .invoice-header h1 {{ margin: 0; color: #98B600; }}
        .invoice-info {{ margin-bottom: 20px; }}
        .invoice-info p {{ margin: 5px 0; font-size: 0.9rem; color: #333; }}
        .invoice-table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        .invoice-table th, .invoice-table td {{ text-align: left; padding: 10px; border: 1px solid #ddd; }}
        .invoice-table th {{ background-color: #094152; color: #ffffff; }}
        .invoice-footer {{ text-align: center; font-size: 0.8rem; color: #777; border-top: 1px solid #ddd; padding-top: 10px; }}
    </style>
    <title>Resumen de Facturas</title>
</head>
<body>
    <div class='invoice-container'>
        <header class='invoice-header'>
            <h1>Resumen de Facturas</h1>
            <p>Detalle de facturas pendientes y procesadas.</p>
        </header>
        <section class='invoice-info'>
            <h2>Información del Inquilino</h2>
            <p><strong>Nombre:</strong> {nombreInquilino}</p>
            <p><strong>Correo:</strong> {correoInquilino}</p>
        </section>
        <table class='invoice-table'>
            <thead>
                <tr>
                    <th>Inmueble</th>
                    <th>Fecha de Pago</th>
                    <th>Estado</th>
                    <th>Monto</th>
                </tr>
            </thead>
            <tbody>
                {rowsBuilder}
            </tbody>
        </table>
        <footer class='invoice-footer'>
            <p>Si tiene alguna pregunta, no dude en contactarnos: soporte@example.com</p>
            <p>© 2025 Aparta+. Todos los derechos reservados.</p>
        </footer>
    </div>
</body>
</html>";
        }


        public async Task EnviarCorreoRecordatorioPagoAsync(string email, string nombreInquilino, string apartamentoId)
        {
            // Construir la URL con el parámetro del apartamento
            var urlHubPagos = $"https://apartamas.com/hub-de-pagos?id={apartamentoId}";

            // Plantilla HTML embebida para el recordatorio de pago
            var htmlTemplate = @"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
    body {
        font-family: Poppins, sans-serif;
        margin: 0;
        padding: 0;
        background-color: #f9f9f9;
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 100vh;
    }
    
    .invoice-container {
        max-width: 600px;
        margin: 20px auto;
        background: #ffffff;
        padding: 20px;
        border-radius: 8px;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    }
    
    .invoice-header {
        text-align: center;
        border-bottom: 2px solid #094152;
        margin-bottom: 20px;
        padding-bottom: 10px;
    }
    
    .invoice-header h1 {
        margin: 0;
        color: #98B600;
    }
    
    .invoice-info {
        display: flex;
        justify-content: space-between;
        margin-bottom: 20px;
    }
    
    .invoice-info div {
        flex: 1;
    }
    
    .invoice-info h2 {
        font-weight: bold;
        margin: 0 0 10px;
        font-size: 1rem;
        color: #555;
    }
    
    .invoice-info p {
        margin: 5px 0;
        font-size: 0.9rem;
        color: #333;
    }
    
    .invoice-table {
        width: 100%;
        border-collapse: collapse;
        margin-bottom: 20px;
    }
    
    .invoice-table th, .invoice-table td {
        text-align: left;
        padding: 10px;
        border: 1px solid #ddd;
    }
    
    .invoice-table th {
        background-color: #094152;
        color: #ffffff;
    }
    
    .invoice-table tfoot td {
        font-weight: normal;
        text-align: right;
        border-top: 2px solid #094152;
    }
    
    .invoice-footer {
        text-align: center;
        font-size: 0.8rem;
        color: #777;
        border-top: 1px solid #ddd;
        padding-top: 10px;
    }
    
    .button {
        background-color: #98B600;
        color: white;
        padding: 10px 20px;
        text-decoration: none;
        border-radius: 4px;
        display: inline-block;
        margin-top: 20px;
    }
    </style>
    <title>Recordatorio de Pago</title>
</head>
<body>
    <div class='invoice-container'>
        <header class='invoice-header'>
            <h1>Recordatorio de Pago - Aparta+</h1>
            <p>Gracias por su preferencia. Recuerde que su pago está pendiente.</p>
        </header>
        <section class='invoice-info'>
            <div>
                <h2>Información del Inquilino</h2>
                <p><strong>Nombre:</strong> [Nombre del Inquilino]</p>
            </div>
            <div>
                <h2>Información de Pago</h2>
                <p><strong>Fecha de vencimiento:</strong> [Fecha de vencimiento]</p>
                <p><strong>Monto pendiente:</strong> [Monto pendiente]</p>
            </div>
        </section>
        <a href='[URL_DEL_HUB_DE_PAGOS]' class='button'>Ir al Hub de Pagos</a>
        <footer class='invoice-footer'>
            <p>Si tiene alguna pregunta, no dude en contactarnos: soporte@apartamas.com</p>
            <p>© 2025 Aparta+. Todos los derechos reservados.</p>
        </footer>
    </div>
</body>
</html>
";

            // Personalizar la plantilla con el nombre del inquilino y la URL del hub de pagos
            var contenidoPersonalizado = htmlTemplate
                .Replace("[Nombre del Inquilino]", nombreInquilino)
                .Replace("[URL_DEL_HUB_DE_PAGOS]", urlHubPagos);

            // Enviar el correo de recordatorio de pago
            await SendHtmlEmailAsync(email, "Recordatorio de Pago - Aparta+", contenidoPersonalizado);
        }
    }
    }
