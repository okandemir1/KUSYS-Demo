using KUSYS.Data.Repository;
using KUSYS.Model;
using Microsoft.EntityFrameworkCore;

namespace KUSYS.Business.Interfaces
{
    public class RoleService : IRoleService
    {
        private readonly IStudentService _studentService;
        private readonly IRepository<RoleClaim> _roleClaimRepository;
        private readonly IRepository<Role> _roleRepository;
        public RoleService(IStudentService _studentService, IRepository<RoleClaim> _roleClaimRepository, IRepository<Role> roleRepository)
        {
            this._studentService = _studentService;
            this._roleClaimRepository = _roleClaimRepository;
            _roleRepository = roleRepository;

        }

        public async Task<List<Role>> GetRoles()
        {
            var roles = await _roleRepository.ListQueryableNoTracking
                .Where(x => !x.IsDeleted).ToListAsync();

            return roles;
        }

        public async Task<List<RoleClaim>> GetUserRoleClaims(string studentId)
        {
            var student = await _studentService.GetStudent(studentId);
            if(student == null)
                return new List<RoleClaim>();

            var claims = await _roleClaimRepository.ListQueryableNoTracking
                .Include(x=>x.DefaultClaim)
                .Include(x=>x.Role)
                .Where(x=>x.RoleId == student.RoleId).ToListAsync();

            return claims;
        }
    }
}
