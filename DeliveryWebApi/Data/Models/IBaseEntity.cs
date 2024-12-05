using Microsoft.VisualBasic;

namespace DeliveryWebApi.Data.Models
{
    public interface IBaseEntity
    {
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
    }
}
