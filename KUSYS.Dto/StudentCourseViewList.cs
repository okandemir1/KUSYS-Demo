using KUSYS.Model;

namespace KUSYS.Dto
{
    public class StudentCourseViewList
    {
        public List<Student> Students { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
        public List<Course> Courses { get; set; }
    }
}