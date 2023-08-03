using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class StudentCourse : BaseEntityWithDateAndId
    {
        public string StudentId { get; set; }
        public string CourseId { get; set; }
        public bool IsActive { get; set; }
    }
}