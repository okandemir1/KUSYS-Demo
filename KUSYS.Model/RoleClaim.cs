using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class RoleClaim : BaseEntityWithDateAndId
    {
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; } 
        public Guid DefaultClaimId { get; set; }
        public virtual DefaultClaim DefaultClaim { get; set; }
    }
}