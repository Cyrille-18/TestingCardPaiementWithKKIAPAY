// <copyright file="EmailTemplates.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paiementapi.Templates
{
    /// <summary>
    /// Provides static methods for generating email templates.
    /// </summary>
    public static class EmailTemplates
    {
        /// <summary>
        /// Generates an HTML email template for email confirmation.
        /// </summary>
        /// <param name="userName">The name of the user to address in the email.</param>
        /// <param name="confirmationLink">The confirmation link to include in the email.</param>
        /// <returns>A formatted HTML email template as a string.</returns>
        public static string GetEmailConfirmationTemplate(string userName, string confirmationLink)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset='UTF-8'>
            <title>Confirmez votre email</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin:0; padding:0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 40px auto;
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 10px;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                }}
                .header {{
                    background-color:rgb(18, 54, 121);
                    color: white;
                    padding: 15px 20px;
                    border-radius: 10px 10px 0 0;
                    text-align: center;
                }}
                .content {{
                    padding: 20px;
                    text-align: center;
                }}
                .btn {{
                    display: inline-block;
                    padding: 12px 25px;
                    margin-top: 20px;
                    font-size: 16px;
                    color: white;
                    background-color:rgb(18, 54, 121);
                    text-decoration: none;
                    border-radius: 5px;
                }}
                .footer {{
                    font-size: 12px;
                    color: #888888;
                    margin-top: 30px;
                    text-align: center;
                }}
            </style>
            </head>
            <body>
            <div class='container'>
                <div class='header'>
                <h2>TestingKKIAPAYCardPaiement</h2>
                </div>
                <div class='content'>
                <h3>Bonjour {userName} !</h3>
                <p>Merci de vous être inscrit sur notre plateforme. Pour activer votre compte, cliquez sur le bouton ci-dessous :</p>
                <a href='{confirmationLink}' class='btn'>Confirmer mon email</a>
                </div>
                <div class='footer'>
                <p>Ce lien est valable pendant 24 heures.</p>
                <p>Si vous n'avez pas créé de compte sur TestingKKIAPAYCardPaiement, ignorez cet email.</p>
                <p>&copy; 2025 TestingKKIAPAYCardPaiement. Tous droits réservés.</p>
                </div>
            </div>
            </body>
            </html>";
        }
    }
}
