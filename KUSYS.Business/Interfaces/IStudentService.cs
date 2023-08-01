using KUSYS.Data.Repository;
using KUSYS.Dto;
using KUSYS.Model;

namespace KUSYS.Business.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetStudents();
        Task<DbOperationResult> Add(StudentActionDto mDto);
        Task<DbOperationResult> Edit(StudentActionDto mDto);
        Task<DbOperationResult> Delete(string id);
        Task<DbOperationResult> Login(string username, string password);
        Task<bool> ExistUsername(string username, string studentId="");
        Task<Student> PasswordCheck(string username, string password, string studentId = "");
    }
}
