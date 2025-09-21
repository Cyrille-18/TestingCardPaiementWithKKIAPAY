// <copyright file="JwtHelper.cs" company="Cyrille-18">
// Copyright (c) Cyrille-18. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace paiementapi.Helpers
{
    /// <summary>
    /// Provides helper methods for generating JSON Web Tokens (JWT) for authentication.
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtHelper"/> class with the specified configuration.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Generates a JWT token for the specified user and roles.
        /// </summary>
        /// <param name="user">The identity user for whom the token is generated.</param>
        /// <param name="roles">A list of roles assigned to the user.</param>
        /// <returns>A JWT token as a string.</returns>
        public string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
