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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using paiementapi.Options;

namespace paiementapi.Helpers
{
    /// <summary>
    /// Provides helper methods for generating JSON Web Tokens (JWT) for authentication.
    /// </summary>
    public class JwtHelper
    {
        private readonly JwtOptions _jwtOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtHelper"/> class with the specified JWT options.
        /// </summary>
        /// <param name="jwtOptions">The JWT configuration options.</param>
        public JwtHelper(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
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

            if (string.IsNullOrEmpty(_jwtOptions.Key))
            {
                throw new InvalidOperationException("JWT Key is not configured. Please set JWT:Key in your configuration.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
