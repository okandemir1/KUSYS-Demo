using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS.Dto
{
    public class StudentSimpleDto
    {
        public string StudentId { get; set; }
        public string Fullname { get; set; }
        public Guid RoleId { get; set; }
    }
}