using CRM.SocialDepartment.Domain.Entities.Assignments;
using DDD.Repositories;

namespace CRM.SocialDepartment.Infrastructure.Interfaces.Repositories;

public interface IAssignmentRepository : IRepository<Assignment>
{
    /// <summary>
    /// Создать новую задачу
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task InsertAsync(Assignment entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновить задачу
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task UpdateAsync(Assignment entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить задачу
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DeleteAsync(Assignment entity, CancellationToken cancellationToken = default);
}