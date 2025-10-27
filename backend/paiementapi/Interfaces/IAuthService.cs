// <copyright file="IAuthService.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using paiementapi.Dtos.auth;

    /// <summary>
    /// Provides authentication services for users and administrators.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        Task RegisterAsync(UserRegisterDto dto);

        /// <summary>
        /// Logs in a user asynchronously.
        /// </summary>
        /// <param name="dto">The user login data transfer object.</param>
        /// <returns>An authentication response DTO.</returns>
        Task<AuthResponseDto> LoginAsync(UserLoginDto dto);

        /// <summary>
        /// Registers a new administrator asynchronously.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        /// <returns>An authentication response DTO.</returns>
        Task<AuthResponseDto> RegisterAdminAsync(UserRegisterDto dto);

        /// <summary>
        /// Confirms the user's email address using a user ID and a token.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is being confirmed.</param>
        /// <param name="encodedToken">The encoded token received for email confirmation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ConfirmEmailAsync(string userId, string encodedToken);

        /// <summary>
        /// Refreshes an authentication token using a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>An authentication response DTO with new tokens.</returns>
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Logs out the user by revoking the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        Task LogoutAsync(string refreshToken);
    }
}