namespace CRM.SocialDepartment.Domain.Entities
{
    public interface ISoftDelete
    {
        public bool SoftDeleted { get; set; }
    }
}
