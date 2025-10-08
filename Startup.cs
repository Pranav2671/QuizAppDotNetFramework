using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.IdentityModel.Tokens;
using Owin;
using System.Text;

[assembly: OwinStartup(typeof(QuizAppDotNetFramework.Startup))]
namespace QuizAppDotNetFramework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var secret = Encoding.UTF8.GetBytes("Your_Secret_Key_Replace_With_A_Strong_One");

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                }
            });
        }
    }
}
