using KUSYS.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CallCenter.Application.Main.Views.Student.Components.StudentForm
{
    public class StudentFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string action,
            StudentActionDto model)
        {
            ViewBag.Action = action;
            return View(model);
        }
    }
}
