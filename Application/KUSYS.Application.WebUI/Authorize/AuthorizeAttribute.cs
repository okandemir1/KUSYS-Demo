using Microsoft.AspNetCore.Mvc;

namespace KUSYS.Application.WebUI.Authorize
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute() 
            : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { };
        }
    }
}
