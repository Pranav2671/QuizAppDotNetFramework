using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizAppDotNetFramework.Security;

namespace QuizAppDotNetFramework.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                var request = filterContext.HttpContext.Request;
                var authHeader = request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    filterContext.Result = new HttpUnauthorizedResult("Missing or invalid Authorization header.");
                    return;
                }

                // Extract token from header
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var principal = JwtManager.ValidateToken(token);

                if (principal == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult("Invalid or expired token.");
                    return;
                }

                // ✅ Attach identity to HttpContext so it can be used in controllers
                HttpContext.Current.User = principal;
            }
            catch (Exception ex)
            {
                filterContext.Result = new HttpUnauthorizedResult("Authorization failed: " + ex.Message);
            }
        }
    }
}
