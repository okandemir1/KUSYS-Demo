using System.ComponentModel.DataAnnotations;

namespace KUSYS.Model.Base
{
    //string(Custom) Id belirlendi�i i�in burada kullanmad�m
    public class BaseEntity
    {
        public bool IsDeleted { get; set; }
    }
    
    public abstract class BaseEntityWithDate : BaseEntity
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }

    public abstract class BaseEntityWithDateAndId : BaseEntityWithDate
    {
        [Key]
        public Guid Id { get; set; }
    }
}