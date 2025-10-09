using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizAppDotNetFramework.Helpers
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        public JwtAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var cookie = httpContext.Request.Cookies["QuizAppJWT"];
                if (cookie == null)
                {
                    System.Diagnostics.Debug.WriteLine("JwtAuthorize: No JWT cookie found.");
                    return false;
                }

                var claims = JwtHelper.ValidateAndGetClaims(cookie.Value);
                if (claims == null)
                {
                    System.Diagnostics.Debug.WriteLine("JwtAuthorize: JWT validation failed.");
                    return false;
                }

                // Check role if roles are specified
                if (_roles.Length > 0)
                {
                    string userRole = claims.Role ?? "";
                    bool roleMatch = _roles.Any(r => string.Equals(r, userRole, StringComparison.OrdinalIgnoreCase));
                    if (!roleMatch)
                    {
                        System.Diagnostics.Debug.WriteLine($"JwtAuthorize: Role '{userRole}' not allowed.");
                        return false;
                    }
                }

                // Set session variables so controllers can access them
                httpContext.Session["UserId"] = Guid.Parse(claims.UserId);
                httpContext.Session["Username"] = claims.Username;
                httpContext.Session["Role"] = claims.Role;

                System.Diagnostics.Debug.WriteLine($"JwtAuthorize: Authorized → UserId={claims.UserId}, Role={claims.Role}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JwtAuthorize Exception: " + ex.Message);
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            System.Diagnostics.Debug.WriteLine("JwtAuthorize: Unauthorized → redirecting to /Auth/Login");
            filterContext.Result = new RedirectResult("/Auth/Login");
        }
    }
}
