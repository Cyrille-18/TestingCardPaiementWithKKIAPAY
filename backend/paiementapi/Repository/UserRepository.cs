// <copyright file="UserRepository.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

namespace paiementapi.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using paiementapi.Interfaces;

    /// <summary>
    /// Repository for managing user-related operations.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="userManager">The user manager for IdentityUser.</param>
        /// <param name="roleManager">The role manager for IdentityRole.</param>
        public UserRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
        public Task<IdentityUser?> GetByIdAsync(string id)
        {
            return this._userManager.FindByIdAsync(id);
        }

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
        public Task<IdentityUser?> GetByEmailAsync(string email)
        {
            return this._userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Creates a new user with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
        /// <exception cref="Exception">Thrown when user creation fails.</exception>
        public async Task<IdentityUser> CreateAsync(IdentityUser user, string password)
        {
            var result = await this._userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        /// <summary>
        /// Gets the roles assigned to the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user whose roles to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of role names.</returns>
        public Task<IList<string>> GetRolesAsync(IdentityUser user)
            => this._userManager.GetRolesAsync(user);

        /// <summary>
        /// Adds the specified user to a role asynchronously.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The role name.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddToRoleAsync(IdentityUser user, string role)
        {
            if (!await this._roleManager.RoleExistsAsync(role))
            {
                await this._roleManager.CreateAsync(new IdentityRole(role));
            }

            await this._userManager.AddToRoleAsync(user, role);
        }

        /// <summary>
        /// Checks if the specified password is valid for the given user asynchronously.
        /// </summary>
        /// <param name="user">The user whose password is to be checked.</param>
        /// <param name="password">The password to validate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if the password is valid; otherwise, false.</returns>
        public Task<bool> CheckPasswordAsync(IdentityUser user, string password)
            => this._userManager.CheckPasswordAsync(user, password);

        /// <summary>
        /// Generates an email confirmation token for the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom to generate the email confirmation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the email confirmation token as a string.</returns>
        public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
        {
            return await this._userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        /// <summary>
        /// Confirms the email address of the specified user using the provided confirmation token asynchronously.
        /// </summary>
        /// <param name="user">The user whose email is to be confirmed.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the email confirmation operation.</returns>
        public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
        {
            return await this._userManager.ConfirmEmailAsync(user, token);
        }
    }
}