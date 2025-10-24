// <copyright file="RefreshTokenService.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using paiementapi.Data;
    using paiementapi.Models;

    /// <summary>
    /// Service for managing refresh tokens.
    /// </summary>
    public class RefreshTokenService
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public RefreshTokenService(ApplicationDBContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Creates a new refresh token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom to create the refresh token.</param>
        /// <param name="expirationDays">The number of days until the token expires.</param>
        /// <returns>The created refresh token.</returns>
        public async Task<RefreshToken> CreateRefreshTokenAsync(IdentityUser user, int expirationDays = 7)
        {
            // Revoke all existing refresh tokens for this user
            await this.RevokeAllRefreshTokensForUserAsync(user.Id);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = this.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays)
            };

            this._context.RefreshTokens.Add(refreshToken);
            await this._context.SaveChangesAsync();

            return refreshToken;
        }

        /// <summary>
        /// Validates a refresh token and returns the associated user if valid.
        /// </summary>
        /// <param name="token">The refresh token to validate.</param>
        /// <returns>The user associated with the valid refresh token, or null if invalid.</returns>
        public async Task<IdentityUser> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await this._context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.IsActive);

            if (refreshToken == null)
            {
                return null;
            }

            // Get the user by ID
            var user = await this._context.Users
                .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);

            if (user == null)
            {
                return null;
            }

            // Mark the used token as revoked
            refreshToken.IsRevoked = true;
            await this._context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Revokes all refresh tokens for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        public async Task RevokeAllRefreshTokensForUserAsync(string userId)
        {
            var tokens = await this._context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await this._context.SaveChangesAsync();
        }

        /// <summary>
        /// Cleans up expired refresh tokens.
        /// </summary>
        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await this._context.RefreshTokens
                .Where(rt => rt.IsExpired)
                .ToListAsync();

            this._context.RefreshTokens.RemoveRange(expiredTokens);
            await this._context.SaveChangesAsync();
        }

        /// <summary>
        /// Generates a cryptographically secure refresh token.
        /// </summary>
        /// <returns>A secure refresh token string.</returns>
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        }
    }
}
