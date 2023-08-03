using KUSYS.Data.Repository;
using KUSYS.Dto;
using KUSYS.Model;

namespace KUSYS.Business.Interfaces
{
    public interface ICourseService
    {
        Task<List<Course>> GetCoursesByIdList(List<string> ids);
        Task<List<Course>> GetAllCourse();
        Task<List<StudentCourseList>> GetStudentCourses(string studentId, bool isOperation = false);
        Task<DbOperationResult> AddStudentCourse(List<string> courseIds, string studentId, bool isOperation = false);
    }
}
