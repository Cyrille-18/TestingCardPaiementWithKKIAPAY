// <copyright file="AuthController.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using paiementapi.Dtos.auth;
    using paiementapi.Interfaces;

    /// <summary>
    /// Controller responsible for authentication actions such as register and login.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service to use.</param>
        public AuthController(IAuthService authService)
        {
            this._authService = authService;
        }

        /// <summary>
        /// Registers a new user with the provided registration data.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the registration process.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            try
            {
                await this._authService.RegisterAsync(dto);
                return this.Ok(new { Message = "Vérifiez votre boîte mail pour confirmer votre compte." });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new admin user with the provided registration data.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the admin creation process.</returns>
        [HttpPost("create-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin(UserRegisterDto dto)
        {
            try
            {
                var result = await this._authService.RegisterAdminAsync(dto);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Confirms a user's email address using the provided user ID and encoded token.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is being confirmed.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>An IActionResult indicating the result of the email confirmation process.</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                await this._authService.ConfirmEmailAsync(userId, token);
                
                return Ok(new { message = "Votre email a été confirmé." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Impossible de confirmer l'email." });
            }
        }

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="dto">The user login data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the login process.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
            {
                var result = await this._authService.LoginAsync(dto);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                // Handle email confirmation case specifically
                if (ex.Message.Contains("confirmer votre adresse email"))
                {
                    return this.BadRequest(ex.Message);
                }
                return this.BadRequest("Email ou mot de passe incorrect");
            }
        }

        /// <summary>
        /// Refreshes an authentication token using a refresh token.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the token refresh process.</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            { 
                var result = await this._authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest("Session expirée, veuillez vous reconnecter");
            }
        }
    }
}