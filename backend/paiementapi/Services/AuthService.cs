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
        private readonly RefreshTokenService _refreshTokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="userRepo">The user repository instance.</param>
        /// <param name="jwt">The JWT helper instance.</param>
        /// <param name="emailService">The email service instance.</param>
        /// <param name="refreshTokenService">The refresh token service instance.</param>
        public AuthService(IUserRepository userRepo, JwtHelper jwt, IEmailService emailService, RefreshTokenService refreshTokenService)
        {
            this._userRepo = userRepo;
            this._jwt = jwt;
            this._emailService = emailService;
            this._refreshTokenService = refreshTokenService;
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
                throw new Exception("Un compte avec cet email existe déjà");
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
                              ?? throw new Exception("FRONTEND_URL est manquant");
            var confirmationLink = $"{frontendUrl}/email-confirmed?userId={user.Id}&token={encodedToken}";

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
                throw new Exception("Un compte avec cet email existe déjà");
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

            // Generate refresh token for admin
            var refreshToken = await this._refreshTokenService.CreateRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                token = token,
                refreshToken = refreshToken.Token,
                user = new UserDto
                {
                    id = user.Id,
                    name = user.UserName,
                    email = user.Email,
                    emailConfirmed = user.EmailConfirmed
                }
            };
        }

        /// <summary>
        /// Confirms a user's email address using the provided user ID and encoded token.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is to be confirmed.</param>
        /// <param name="encodedToken">The encoded email confirmation token.</param>
        public async Task ConfirmEmailAsync(string userId, string encodedToken)
        {
            var user = await this._userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Lien de confirmation invalide");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
            var result = await this._userRepo.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                throw new Exception("Erreur lors de la confirmation de l'email");
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
                throw new Exception("Email ou mot de passe incorrect");
            }

            if (!user.EmailConfirmed)
            {
                throw new Exception("Veuillez confirmer votre adresse email avant de vous connecter");
            }

            var roles = await this._userRepo.GetRolesAsync(user);
            var token = this._jwt.GenerateJwtToken(user, roles);

            // Generate refresh token
            var refreshToken = await this._refreshTokenService.CreateRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                token = token,
                refreshToken = refreshToken.Token,
                user = new UserDto
                {
                    id = user.Id,
                    name = user.UserName,
                    email = user.Email,
                    emailConfirmed = user.EmailConfirmed
                }
            };
        }

        /// <summary>
        /// Refreshes an authentication token using a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>An authentication response DTO with new tokens.</returns>
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // Validate the refresh token and get the user
            var user = await this._refreshTokenService.ValidateRefreshTokenAsync(refreshToken);
            
            if (user == null)
            {
                throw new Exception("Session expirée, veuillez vous reconnecter");
            }

            // Generate new JWT token
            var roles = await this._userRepo.GetRolesAsync(user);
            var newToken = this._jwt.GenerateJwtToken(user, roles);

            // Generate new refresh token
            var newRefreshToken = await this._refreshTokenService.CreateRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                token = newToken,
                refreshToken = newRefreshToken.Token,
                user = new UserDto
                {
                    id = user.Id,
                    name = user.UserName,
                    email = user.Email,
                    emailConfirmed = user.EmailConfirmed
                }
            };
        }
    }
}