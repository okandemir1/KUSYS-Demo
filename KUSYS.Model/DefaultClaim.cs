using KUSYS.Model.Base;

namespace KUSYS.Model
{
    public class DefaultClaim : BaseEntityWithDateAndId
    {
        public string Label { get; set; }
        public string UserRight { get; set; }
    }
}