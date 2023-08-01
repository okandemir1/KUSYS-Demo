using KUSYS.Application.WebUI.Helpers;
using KUSYS.Business.Interfaces;
using KUSYS.Dto;
using KUSYS.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KUSYS.Application.WebUI.Authorize
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        private readonly IStudentService _studentService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClaimRequirementFilter(IHttpContextAccessor contextAccessor, IStudentService _studentService, IRoleService _roleService)
        {
            _contextAccessor = contextAccessor;
            this._studentService = _studentService;
            this._roleService = _roleService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role);
            if (role == null)
            {
                context.Result = new RedirectResult("/Auth/Login");
                return;
            }

            var session = new SessionHelper(_contextAccessor);
            var studentSession = session.Get<StudentSimpleDto>("Student");

            var studentId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            if (studentSession == null)
            {
                var student = _studentService.GetStudent(studentId).GetAwaiter().GetResult();
                if (student == null)
                {
                    context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Result = new RedirectResult("/Auth/Login");
                    return;
                }
                session.Set("Student", student);
            }

            var sessionStudentClaims = session.Get<List<RoleClaim>>("StudentClaims");

            if (sessionStudentClaims == null || !sessionStudentClaims.Any())
            {
                List<RoleClaim> roleClaims = _roleService.GetUserRoleClaims(studentId).GetAwaiter().GetResult();
                session.Set("StudentClaims", roleClaims);
            }

            return;
        }
    }
}
