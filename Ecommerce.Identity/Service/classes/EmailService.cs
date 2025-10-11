using System.Net;
using System.Net.Mail;
using System.Web;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Identity.Service.classes
{
    public class EmailService : IEmailService
    {
        private readonly string? _smtpServer;
        private readonly int _smtpPort;
        private readonly string? _username;
        private readonly string? _password;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:Smtp"];
            _smtpPort = int.TryParse(configuration["Email:SmtpPort"], out var port) ? port : 587;
            _username = configuration["Email:Username"];
            _password = configuration["Email:Password"];
        }

        public async Task SendEmailAsync(string to, string userId, string token, HttpRequest request, string? bodyContent = null)
        {
            if (string.IsNullOrEmpty(_smtpServer) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                throw new InvalidOperationException("Email configuration is invalid.");

            string tokenEncoding = HttpUtility.UrlEncode(token);
            string confirmUrl = $"{request.Scheme}://{request.Host}/api/Customer/Auth/ConfirmEmail?userId={userId}&token={tokenEncoding}";

            string subject = "Confirm your email address";

            // Default professional body if bodyContent is null or empty
            string defaultBody = $@"
                <p>Hello,</p>
                <p>Thank you for signing up with <strong>Your E-Commerce Store</strong>! 
                Please confirm your email address by clicking the button below.</p>
                <p><a href='{confirmUrl}' class='button'>Confirm Email</a></p>";

            string finalBodyContent = string.IsNullOrWhiteSpace(bodyContent) ? defaultBody : bodyContent;

            // HTML Email Template with Stylish Design
            string htmlBody = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f6f8; margin:0; padding:0; }}
                    .container {{ max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); overflow: hidden; }}
                    .header {{ background-color: #0078D7; color: white; padding: 20px; text-align: center; }}
                    .header img {{ max-height: 50px; margin-bottom: 10px; }}
                    .content {{ padding: 30px; color: #333333; line-height: 1.6; }}
                    .button {{ display: inline-block; padding: 12px 25px; background-color: #0078D7; color: #ffffff !important; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                    .footer {{ background-color: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #777777; }}
                    h2 {{ margin-top: 0; }}
                    p {{ margin: 15px 0; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='https://yourstore.com/logo.png' alt='Your Store Logo'/>
                        <h2>Your E-Commerce Store</h2>
                    </div>
                    <div class='content'>
                        {finalBodyContent}
                    </div>
                    <div class='footer'>
                        &copy; {DateTime.Now.Year} Your E-Commerce Store. All rights reserved.
                    </div>
                </div>
            </body>
            </html>";

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            var mail = new MailMessage(_username, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            try
            {
                await client.SendMailAsync(mail);
                Console.WriteLine("✅ Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                throw;
            }
        }

        public async Task SendOrderConfirmationEmailAsync(string to, string subject, string htmlContent)
        {
            if (string.IsNullOrEmpty(_smtpServer) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                throw new InvalidOperationException("Email configuration is invalid.");

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            var mail = new MailMessage(_username, to, subject, htmlContent)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }
    }
}
