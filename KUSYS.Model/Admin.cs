using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class Admin : BaseEntityWithDateAndId
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}