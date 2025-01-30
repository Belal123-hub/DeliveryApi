namespace DAL.Data
{
    public class LogoutUser : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public DateTime? DeleteDateTime { get; set; }
        public Guid Identifier { get; set; }
    }
}
