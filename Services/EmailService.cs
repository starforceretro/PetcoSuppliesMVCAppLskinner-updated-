using Microsoft.Extensions.Configuration;
using PetcoSuppliesMVCAppLskinner.Models;
using System.Net;
using System.Net.Mail;

namespace PetcoSuppliesMVCAppLskinner.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration; // configuration to access email settings from appsettings.json

        public EmailService(IConfiguration configuration) // constructor with dependency injection to get the configuration
        {
            _configuration = configuration; // assign the injected configuration to the private field
        }

        public async Task SendContactEmail( // method to send a contact email, accepts the customer's name, email, subject, and message as parameters
    string customerName,
    string customerEmail,
    string subject,
    string message)
        {
            var email =
                _configuration["EmailSettings:Email"]; /// email address to send from, retrieved from the configuration settings (appsettings.json)

            var password =
                _configuration["EmailSettings:Password"];

            var host =
                _configuration["EmailSettings:Host"];

            var port =
                int.Parse(_configuration["EmailSettings:Port"]); // port number for the SMTP server, retrieved from the configuration settings (appsettings.json) and parsed as an integer

            using var client = new SmtpClient(host, port) // create a new SmtpClient instance with the specified host and port
            {
                Credentials =
                    new NetworkCredential(email, password),

                EnableSsl = true
            };

            var mailMessage = new MailMessage // create a new MailMessage instance to represent the email being sent
            {
                From = new MailAddress(email),

                Subject = $"Customer Contact: {subject}",

                Body = $@"
            <h2>New Customer Message</h2>

            <p>
                <strong>Name:</strong>
                {customerName}
            </p>

            <p>
                <strong>Email:</strong>
                {customerEmail}
            </p>

            <hr />

            <p>
                {message}
            </p>
        ",

                IsBodyHtml = true
            };

            // gets sent to the petco email

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }

        public async Task SendOrderConfirmationEmail( // method to send an order confirmation email, accepts the customer's email and the order details as parameters
            string toEmail,
            Order order)
        {
            var email =
                _configuration["EmailSettings:Email"];

            var password =
                _configuration["EmailSettings:Password"];

            var host =
                _configuration["EmailSettings:Host"];

            var port =
                int.Parse(_configuration["EmailSettings:Port"]);

            var subject = // subject of the email, including the order ID for reference
                $"Order Confirmation #{order.Id}";

            var body = $@"
                <h2>Thank you for your order!</h2>

                <p>
                    Your order has been placed successfully.
                </p>

                <hr />

                <p>
                    <strong>Order Number:</strong> {order.Id}
                </p>

                <p>
                    <strong>Date:</strong> {order.OrderDate}
                </p>

                <p>
                    <strong>Total:</strong> £{order.TotalAmount}
                </p>

                <p>
                    <strong>Status:</strong> {order.Status}
                </p>
            ";

            using var client = new SmtpClient(host, port) // create a new SmtpClient instance with the specified host and port
            {
                Credentials =
                    new NetworkCredential(email, password),

                EnableSsl = true
            };

            var message = new MailMessage // create a new MailMessage instance to represent the order confirmation email being sent
            {
                From = new MailAddress(email),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message); // send the order confirmation email to the customer's email address
        }
    }
}