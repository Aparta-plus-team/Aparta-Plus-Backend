using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Utils
{
    public class SNSService
    {
        private readonly IAmazonSimpleEmailService _sesClient;
        private readonly string _fromEmail;
        private readonly ILogger<SNSService> _logger;

        public SNSService(IConfiguration configuration, ILogger<SNSService> logger)
        {
            _logger = logger;

            var awsKey = configuration["AWS:AccessKey"];
            var awsSecret = configuration["AWS:SecretKey"];
            var region = configuration["AWS:Region"];
            _fromEmail = configuration["AWS:FromEmail"];

            _sesClient = new AmazonSimpleEmailServiceClient(awsKey, awsSecret, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string redirectUrl)
        {
            try
            {
                var htmlContent = GenerateEmailTemplate(redirectUrl);

                var sendRequest = new SendEmailRequest
                {
                    Source = _fromEmail,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { toEmail }
                    },
                    Message = new Amazon.SimpleEmail.Model.Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = htmlContent
                            }
                        }
                    }
                };

                var response = await _sesClient.SendEmailAsync(sendRequest);
                _logger.LogInformation($"Email sent successfully. MessageId: {response.MessageId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {toEmail}");
                throw;
            }
        }

        private string GenerateEmailTemplate(string redirectUrl)
        {
            return @$"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                .container {{ 
                    font-family: Arial, sans-serif;
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background-color: #f8f9fa;
                }}
                .header {{
                    background-color: #007bff;
                    color: white;
                    padding: 20px;
                    text-align: center;
                    border-radius: 5px 5px 0 0;
                }}
                .content {{
                    background-color: white;
                    padding: 20px;
                    border-radius: 0 0 5px 5px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                }}
                .button {{
                    display: inline-block;
                    padding: 12px 24px;
                    background-color: #007bff;
                    color: white;
                    text-decoration: none;
                    border-radius: 5px;
                    margin-top: 20px;
                    font-weight: bold;
                }}
                .button:hover {{
                    background-color: #0056b3;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1 style='margin: 0;'>Notificación</h1>
                </div>
                <div class='content'>
                    <p>Haga clic en el siguiente botón para continuar:</p>
                    <a href='{redirectUrl}' class='button'>Continuar</a>
                </div>
            </div>
        </body>
        </html>";
        }
    }
}
