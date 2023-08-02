using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class StudentCourse : BaseEntity
    {
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}