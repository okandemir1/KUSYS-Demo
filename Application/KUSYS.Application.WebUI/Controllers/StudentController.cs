using Microsoft.AspNetCore.Mvc;
using KUSYS.Application.WebUI.Authorize;
using KUSYS.Business.Interfaces;
using KUSYS.Business.Filters;
using KUSYS.Dto;
using KUSYS.Application.WebUI.Helpers;
using KUSYS.Model;

namespace KUSYS.Application.WebUI.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        SessionHelper _session;
        List<RoleClaim> claims;
        public StudentController(IStudentService _studentService, SessionHelper _session)
        {
            this._studentService = _studentService;
            this._session = _session;
            claims = _session.Get<List<RoleClaim>>("StudentClaims");
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
                return Json(
                new
                {
                    draw = dataTableParameters.Draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<StudentSimpleDto>()
                });

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
    }
}
