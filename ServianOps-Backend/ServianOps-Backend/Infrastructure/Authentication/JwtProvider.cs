using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServianOps_Backend.Application.UserModule.User.UserDto;
using ServianOps_Backend.Application.Interfaces;

namespace ServianOps_Backend.Infrastructure.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtSettings _jwtSettings;

        public JwtProvider(IOptions<JwtSettings> jwtSettingsOptions)
        {
            _jwtSettings = jwtSettingsOptions.Value;
        }

        public string GenerateToken(UserDetailDto user, string roleName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                // Custom Multi-Tenant claims
                new Claim("tenant_id", user.TenantId.ToString()),
                new Claim("user_id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
