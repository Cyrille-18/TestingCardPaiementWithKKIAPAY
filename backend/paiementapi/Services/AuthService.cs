// <copyright file="AuthService.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.WebUtilities;
    using paiementapi.Dtos.auth;
    using paiementapi.Helpers;
    using paiementapi.Interfaces;
    using paiementapi.Templates;

    /// <summary>
    /// Provides authentication services such as user registration and login.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtHelper _jwt;

        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="userRepo">The user repository instance.</param>
        /// <param name="jwt">The JWT helper instance.</param>
        /// <param name="emailService">The email service instance.</param>
        public AuthService(IUserRepository userRepo, JwtHelper jwt, IEmailService emailService)
        {
            this._userRepo = userRepo;
            this._jwt = jwt;
            this._emailService = emailService;
        }

        /// <summary>
        /// Registers a new user with the provided registration data.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        public async Task RegisterAsync(UserRegisterDto dto)
        {
            var existingUser = await this._userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("Cet email est déjà utilisé");
            }

            var user = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
            };

            await this._userRepo.CreateAsync(user, dto.Password);
            await this._userRepo.AddToRoleAsync(user, "User");

            // Send confirmation email
            var token = await this._userRepo.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
                              ?? throw new Exception("FRONTEND_URL est manquant dans .env");
            var confirmationLink = $"{frontendUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            var html = EmailTemplates.GetEmailConfirmationTemplate(user.UserName, confirmationLink);
            await this._emailService.SendEmailAsync(user.Email, "Confirmez votre email", html);
        }

        /// <summary>
        /// Registers a new admin user with the provided registration data.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        /// <returns>An authentication response containing the JWT token and username.</returns>
        public async Task<AuthResponseDto> RegisterAdminAsync(UserRegisterDto dto)
        {
            var existingUser = await this._userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("Cet email est déjà utilisé");
            }

            var user = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
            };

            await this._userRepo.CreateAsync(user, dto.Password);

            await this._userRepo.AddToRoleAsync(user, "Admin");

            var roles = await this._userRepo.GetRolesAsync(user);
            var token = this._jwt.GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.UserName,
            };
        }

        /// <summary>
        /// Confirms a user's email address using the provided user ID and encoded token.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is to be confirmed.</param>
        /// <param name="encodedToken">The encoded email confirmation token.</param>
        public async Task ConfirmEmailAsync(string userId, string encodedToken)
        {
            var user = await this._userRepo.GetByEmailAsync(userId);
            if (user == null)
            {
                throw new Exception("Lien invalide");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
            var result = await this._userRepo.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new Exception("Impossible de confirmer l'email");
            }
        }

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="dto">The user login data transfer object.</param>
        /// <returns>An authentication response containing the JWT token and username.</returns>
        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await this._userRepo.GetByEmailAsync(dto.Email);

            if (user == null || !await this._userRepo.CheckPasswordAsync(user, dto.Password))
            {
                throw new Exception("Identifiants invalides");
            }

            if (!user.EmailConfirmed)
            {
                throw new Exception("Veuillez confirmer votre email avant de vous connecter.");
            }

            var roles = await this._userRepo.GetRolesAsync(user);
            var token = this._jwt.GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.UserName,
            };
        }
    }
}