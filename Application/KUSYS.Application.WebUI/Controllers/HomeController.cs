using Microsoft.AspNetCore.Mvc;
using KUSYS.Application.WebUI.Authorize;
using KUSYS.Business.Interfaces;
using KUSYS.Business.Filters;
using KUSYS.Dto;
using System.Security.Claims;
using KUSYS.Model;
using KUSYS.Application.WebUI.Helpers;
using System.Collections.Generic;

namespace KUSYS.Application.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        SessionHelper _session;
        List<RoleClaim> claims;
        public HomeController(IStudentService _studentService, ICourseService _courseService, SessionHelper _session)
        {
            this._studentService = _studentService;
            this._courseService = _courseService;
            this._session = _session;
            claims = _session.Get<List<RoleClaim>>("StudentClaims");
        }
        public async Task<IActionResult> Index()
        {
            var model = new StudentCourseViewList();
            if (claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                model = await _studentService.GetAllWithCourses();
            else
                model = await _studentService.GetStudentWithCourses(_session.Get<StudentSimpleDto>("Student").StudentId);

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetCourses(string id)
        {
            if (claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
            {
                var list = await _courseService.GetStudentCourses(id,true);
                return Json(new { data = list });
            }
            else
            {
                var list = await _courseService.GetStudentCourses(id, false);
                return Json(new { data = list });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddCourse(List<string> courseList, string studentId)
        {
            if (claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
            {
                var response = await _courseService.AddStudentCourse(courseList, studentId, true);
                return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
            }
            else if (claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("SelectCourse"))
            {
                var response = await _courseService.AddStudentCourse(courseList, studentId, false);
                return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
            }

            return Json(new { isSucceed = false, message = "Yetkiniz yoktur" });
        }
    }
}
