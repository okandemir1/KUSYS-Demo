using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class RoleClaim : BaseEntityWithDateAndId
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } 
        public int DefaultClaimId { get; set; }
        public virtual DefaultClaim DefaultClaim { get; set; }
    }
}