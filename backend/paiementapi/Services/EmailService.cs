// <copyright file="EmailService.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using paiementapi.Interfaces;


    /// <summary>
    /// Service implementation for sending emails using SMTP protocol.
    /// This service provides functionality to send HTML emails through SMTP servers.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// The email address from which emails will be sent.
        /// </summary>
        private readonly string _fromEmail;
        
        /// <summary>
        /// The application password for SMTP authentication.
        /// </summary>
        private readonly string _appPassword;
        
        /// <summary>
        /// The SMTP server host address.
        /// </summary>
        private readonly string _host;
        
        /// <summary>
        /// The SMTP server port number.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// Reads email configuration from environment variables.
        /// </summary>
        /// <exception cref="Exception">Thrown when required environment variables are missing.</exception>
        public EmailService()
        {
            _fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM") ?? throw new Exception("EMAIL_FROM missing in .env");
            _appPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD") ?? throw new Exception("EMAIL_APP_PASSWORD missing in .env");
            _host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? "smtp.gmail.com";
            _port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587");
        }

        /// <summary>
        /// Sends an email asynchronously to the specified recipient.
        /// </summary>
        /// <param name="toEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="htmlContent">The HTML content of the email body.</param>
        /// <returns>A task representing the asynchronous email sending operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null or empty.</exception>
        /// <exception cref="SmtpException">Thrown when there is an error sending the email through SMTP.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the SMTP client is not properly configured.</exception>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            using var smtpClient = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_fromEmail, _appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, "TestingKkiapayCardPaiement"),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}