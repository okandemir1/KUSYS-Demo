using KUSYS.Business.Filters;
using KUSYS.Data.Repository;
using KUSYS.Dto;
using KUSYS.Model;

namespace KUSYS.Business.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetStudents();
        Task<DataTableViewModelResult<List<StudentSimpleDto>>> GetAll(StudentFilterModel filterModel);
        Task<DbOperationResult> Add(StudentActionDto mDto);
        Task<DbOperationResult> Edit(StudentActionDto mDto);
        Task<DbOperationResult> Delete(string id);
        Task<StudentSimpleDto> GetStudent(string id);
        Task<DbOperationResult<StudentSimpleDto>> Login(LoginDto mDto);
        Task<bool> ExistUsername(string username, string studentId="");
        Task<Student> PasswordCheck(string username, string password, string studentId = "");
    }
}
