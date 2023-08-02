using KUSYS.Model;

namespace KUSYS.Business.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleClaim>> GetUserRoleClaims(string studentId);
        Task<List<Role>> GetRoles();

    }
}
