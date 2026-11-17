// <copyright file="IEmailService.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface defining service operations for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="htmlContent">The HTML content of the email.</param>
        /// <returns>A task representing the email sending operation.</returns>
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
    }
}