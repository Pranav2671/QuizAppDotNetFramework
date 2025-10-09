using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace QuizAppDotNetFramework.Helpers
{
    public class JwtClaims
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public static class JwtHelper
    {
        public static readonly string SecretKey = "YourSuperSecretKey1234567890!@#$%^&*()_+"; // must be 256+ bits

        public static string GenerateToken(string userId, string username, string role, int expireHours = 1)
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", userId),
                    new Claim("Username", username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(expireHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static JwtClaims ValidateAndGetClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var key = Encoding.UTF8.GetBytes(SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return new JwtClaims
                {
                    UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value,
                    Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value,
                    Role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
