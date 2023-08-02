using Microsoft.AspNetCore.Mvc;
using KUSYS.Application.WebUI.Authorize;
using KUSYS.Business.Interfaces;
using KUSYS.Business.Filters;
using KUSYS.Dto;
using System.Security.Claims;

namespace KUSYS.Application.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IStudentService _studentService;
        public HomeController(IStudentService _studentService)
        {
            this._studentService = _studentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStudentList(DataTableParameters dataTableParameters)
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
    }
}
