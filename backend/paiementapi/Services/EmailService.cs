// <copyright file="EmailService.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using paiementapi.Interfaces;
    using SendGrid;
    using SendGrid.Helpers.Mail;

    /// <summary>
    /// Service implementation for sending emails using SendGrid API.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// The SendGrid API key for authentication.
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <exception cref="Exception">Thrown when SENDGRID_API_KEY environment variable is not set.</exception>
        public EmailService()
        {
            this._apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
                      ?? throw new Exception("SENDGRID_API_KEY est manquant");
        }

        /// <summary>
        /// Sends an email asynchronously using SendGrid service.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="htmlContent">The HTML content of the email.</param>
        /// <returns>A task representing the email sending operation.</returns>
        /// <exception cref="Exception">Thrown when email sending fails.</exception>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(this._apiKey);
            var from = new EmailAddress("no-reply@testingkKIAPAYCardPaiement.com", "PaiementAPI");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de l'envoi de l'email. Code: {response.StatusCode}");
            }
        }
    }
}