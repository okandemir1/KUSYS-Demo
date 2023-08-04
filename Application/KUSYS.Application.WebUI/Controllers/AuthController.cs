using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using KUSYS.Business.Interfaces;
using KUSYS.Application.WebUI.Helpers;
using KUSYS.Dto;
using KUSYS.Application.WebUI.Authorize;

namespace KUSYS.Application.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IStudentService _studentService;
        SessionHelper _sessionHelper;

        public AuthController(IStudentService _studentService, SessionHelper sessionHelper)
        {
            this._studentService = _studentService;
            _sessionHelper = sessionHelper;
        }

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Demo()
        {
            await _studentService.CreateDemo();
            return Json(new { isSucceed = true, });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto reqModel)
        {
            var loginResult = await _studentService.Login(reqModel);
            if (!loginResult.IsSucceed)
                return Json(new { isSucceed = loginResult.IsSucceed, message = loginResult.Message, errors = loginResult.Errors, instance = "" });

            var student = loginResult.Instance;

            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, student.StudentId),
                        new Claim("FullName", $"{student.Firstname} {student.Lastname}"),
                        new Claim(ClaimTypes.Role, student.RoleId.ToString()),
                    };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IssuedUtc = DateTime.UtcNow,
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(1200),
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _sessionHelper.Set("Student", student);

            return Json(new { isSucceed = loginResult.IsSucceed, message = loginResult.Message, errors = loginResult.Errors, instance = "" });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _sessionHelper.Set("Student", null);
            }
            
            return RedirectToAction("Login","Auth");
        }
    }
}
