using Microsoft.AspNetCore.Mvc;
using KUSYS.Application.WebUI.Authorize;
using KUSYS.Business.Interfaces;
using KUSYS.Business.Filters;
using KUSYS.Dto;
using KUSYS.Application.WebUI.Helpers;
using KUSYS.Model;
using System.Collections.Generic;

namespace KUSYS.Application.WebUI.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        SessionHelper _session;
        List<RoleClaim> claims;
        public StudentController(IStudentService _studentService, SessionHelper _session, ICourseService courseService)
        {
            this._studentService = _studentService;
            this._session = _session;
            claims = _session.Get<List<RoleClaim>>("StudentClaims");
            _courseService = courseService;
        }

        public IActionResult Index()
        {
            if(claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return View();

            return RedirectToAction("/Auth/AccessDenied");
        }

        [HttpPost]
        public async Task<IActionResult> GetList(DataTableParameters dataTableParameters)
        {
            if (claims == null && !claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return Json( new { draw = dataTableParameters.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<StudentSimpleDto>() });

            var response = await _studentService.GetAll(new StudentFilterModel(dataTableParameters));

            return Json(
            new
            {
                draw = dataTableParameters.Draw,
                recordsFiltered = response.RecordsFiltered,
                recordsTotal = response.TotalCount,
                data = response.Data
            });
        }

        public IActionResult Create()
        {
            if (claims == null && !claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return RedirectToAction("/Auth/AccessDenied");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentActionDto model)
        {
            if (claims == null && !claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return RedirectToAction("/Auth/AccessDenied");

            var response = await _studentService.Add(model);
            return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (claims == null && !claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return RedirectToAction("/Auth/AccessDenied");

            var data = await _studentService.GetStudentInfo(id);
            if (data == null)
                return RedirectToAction("Index", "Student", new { q = "not_found_data" });

            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentActionDto model)
        {
            if (claims == null && !claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return RedirectToAction("/Auth/AccessDenied");

            var response = await _studentService.Edit(model);
            return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (claims == null && !claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
                return RedirectToAction("/Auth/AccessDenied");

            var response = await _studentService.Delete(id);
            return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
        }

        [HttpGet]
        public async Task<JsonResult> GetCourses(string id)
        {
            if (claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
            {
                var list = await _courseService.GetStudentCourses(id);
                return Json(new { data = list });
            }
            else
            {
                return Json(new { isSucceed = false, message = "Yetkiniz yoktur" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddCourse(List<string> courseList, string studentId)
        {
            if (claims != null && claims.Select(x => x.DefaultClaim.UserRight).ToList().Contains("StudentManagement"))
            {
                var response = await _courseService.AddStudentCourse(courseList, studentId);
                return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
            }

            return Json(new { isSucceed = false, message = "Yetkiniz yoktur" });
        }
    }
}
