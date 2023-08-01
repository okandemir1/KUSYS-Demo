using KUSYS.Data.Repository;
using KUSYS.Model;
using Microsoft.EntityFrameworkCore;

namespace KUSYS.Business.Interfaces
{
    public class RoleService : IRoleService
    {
        private readonly IStudentService _studentService;
        private readonly IRepository<RoleClaim> _roleClaimRepository;
        public RoleService(IStudentService _studentService, IRepository<RoleClaim> _roleClaimRepository)
        {
            this._studentService = _studentService;
            this._roleClaimRepository = _roleClaimRepository;
        }

        public async Task<List<RoleClaim>> GetUserRoleClaims(string studentId)
        {
            var student = await _studentService.GetStudent(studentId);
            if(student == null)
                return new List<RoleClaim>();

            var claims = await _roleClaimRepository.ListQueryableNoTracking
                .Include(x=>x.Role)
                .Where(x=>x.RoleId == student.RoleId).ToListAsync();

            return claims;
        }
    }
}
