using data_aparta_.Repos.Utils;

namespace aparta_.Types
{
    [MutationType]
    public class EmailMutations
{

        public async Task<string> SendEmail([Service]  EmailService emailService, string toEmail, string subject, string redirectUrl)
        {
            try
            {
                string htmlContent = @"
            <html>
            <head>
                <style>
                    .container { 
                        padding: 20px;
                        background-color: #f0f0f0;
                    }
                    .title {
                        color: #333;
                        font-size: 24px;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1 class='title'>¡Hola!</h1>
                    <p>Este es un correo de prueba con HTML y CSS.</p>
                    <a href='" + redirectUrl + @"'>Haz clic aquí para continuar</a>
                </div>
            </body>
            </html>";

                await emailService.SendHtmlEmailAsync(
                    toEmail,
                    subject,
                    htmlContent
                );

                return "Correo enviado exitosamente";

            }catch (Exception e)
            {
                throw new GraphQLException(e.Message);
            }
          
        }
}
}
