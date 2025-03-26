namespace CRM.SocialDepartment.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message = "Сущность не найдена") : base(message)
        {
        }
    }
}
