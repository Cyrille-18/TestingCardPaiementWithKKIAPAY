// <copyright file="IUserRepository.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace paiementapi.Interfaces
{
    /// <summary>
    /// Interface for user repository operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<IdentityUser?> GetByEmailAsync(string email);

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="id">The user's ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<IdentityUser?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new user with the specified password.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The created user.</returns>
        Task<IdentityUser> CreateAsync(IdentityUser user, string password);

        /// <summary>
        /// Gets the roles assigned to the specified user.
        /// </summary>
        /// <param name="user">The user whose roles to retrieve.</param>
        /// <returns>A list of role names.</returns>
        Task<IList<string>> GetRolesAsync(IdentityUser user);

        /// <summary>
        /// Adds the specified user to a role.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The role name.</param>
        Task AddToRoleAsync(IdentityUser user, string role);

        /// <summary>
        /// Checks if the specified password is correct for the user.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns>True if the password is correct; otherwise, false.</returns>
        Task<bool> CheckPasswordAsync(IdentityUser user, string password);

        /// <summary>
        /// Generates an email confirmation token for the user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>The email confirmation token.</returns>
        Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);

        /// <summary>
        /// Confirms the user's email using the provided token.
        /// </summary>
        /// <param name="user">The user to confirm.</param>
        /// <param name="token">The confirmation token.</param>
        /// <returns>The result of the confirmation operation.</returns>
        Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token);
    }
}