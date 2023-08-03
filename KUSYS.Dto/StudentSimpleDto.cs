using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS.Dto
{
    public class StudentSimpleDto
    {
        public string StudentId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int RoleId { get; set; }
    }
}