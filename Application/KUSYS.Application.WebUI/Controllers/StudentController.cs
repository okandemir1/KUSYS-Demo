using Microsoft.AspNetCore.Mvc;
using KUSYS.Application.WebUI.Authorize;
using KUSYS.Business.Interfaces;
using KUSYS.Business.Filters;
using KUSYS.Dto;

namespace KUSYS.Application.WebUI.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService _studentService)
        {
            this._studentService = _studentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetList(DataTableParameters dataTableParameters)
        {
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentActionDto model)
        {
            var response = await _studentService.Add(model);
            return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var data = _studentService.GetStudent(id);
            if (data == null)
                return RedirectToAction("Index", "Student", new { q = "not_found_data" });

            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentActionDto model)
        {
            var response = await _studentService.Edit(model);
            return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _studentService.Delete(id);
            return Json(new { isSucceed = response.IsSucceed, message = response.Message, errors = response.Errors });
        }
    }
}
