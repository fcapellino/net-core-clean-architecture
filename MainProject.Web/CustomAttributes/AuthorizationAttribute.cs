namespace MainProject.Web.CustomAttributes
{
    using Microsoft.AspNetCore.Authorization;

    internal class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
