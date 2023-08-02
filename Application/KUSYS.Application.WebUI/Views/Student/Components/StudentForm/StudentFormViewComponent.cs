using KUSYS.Business.Interfaces;
using KUSYS.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CallCenter.Application.Main.Views.Student.Components.StudentForm
{
    public class StudentFormViewComponent : ViewComponent
    {
        private readonly IRoleService roleService;
        public StudentFormViewComponent(IRoleService roleService)
        {
            this.roleService = roleService;
        }
        public IViewComponentResult Invoke(string action,
            StudentActionDto model)
        {

            ViewBag.Action = action;
            ViewBag.Roles = roleService.GetRoles().GetAwaiter().GetResult();
            return View(model);
        }
    }
}
