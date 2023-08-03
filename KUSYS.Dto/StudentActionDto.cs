using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS.Dto
{
    public class StudentActionDto
    {
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}