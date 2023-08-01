using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS.Dto
{
    public class StudentPasswordDto
    {
        public string StudentId { get; set; }
        public string Password { get; set; }
        public string RPassword { get; set; }
        public string OldPassword { get; set; }
    }
}