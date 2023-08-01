using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class Course : BaseEntity
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
    }
}