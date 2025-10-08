using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace QuizAppDotNetFramework.Security
{
    public static class JwtManager
    {
        // ⚠️ Use a long, random secret key in production
        private static readonly string SecretKey = "ThisIsASecretKeyForJWT_ChangeThisToSomethingStrong!";

        // Create token with username + role
        public static string GenerateToken(string username, string role, int expireMinutes = 60)
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        // Validate incoming JWT and return user claims
        public static ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(SecretKey);
                var tokenHandler = new JwtSecurityTokenHandler();

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, parameters, out validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
