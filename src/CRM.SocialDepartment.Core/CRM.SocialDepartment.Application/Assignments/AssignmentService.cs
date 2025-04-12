using System.Linq.Expressions;
using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Exceptions;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Application.Assignments;

public class AssignmentService(IMapper mapper, MongoBasicRepository<Assignment, Guid> assignmentRepository)
{
    private readonly MongoBasicRepository<Assignment, Guid> _assignmentRepository = assignmentRepository;

    public IMongoCollection<Assignment> GetAssignmentCollection() => _assignmentRepository.GetCollection();

    public async Task<Assignment?> GetAssignmentByIdAsync(Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        return await _assignmentRepository.GetAsync(e => e.Id == assignmentId, cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync(Expression<Func<Assignment, bool>>? predicate,
        CancellationToken cancellationToken = default)
    {
        return await _assignmentRepository.GetAllAsync(predicate, cancellationToken);
    }

    public async Task<Guid> CreateAssignmentAsync(CreateOrEditAssignmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var assignment = new Assignment(
            dto.AcceptDate,
            dto.DepartmentNumber,
            dto.Description,
            dto.ForwardDate,
            dto.ForwardDeaprtment,
            new()
            {
                { DateTime.Now, "Создано" }
            },
            dto.DepartmentForwardDate,
            dto.Assignee,
            dto.Note,
            DateTime.Now,
            dto.Patient
        );

        await _assignmentRepository.InsertAsync(assignment, cancellationToken);

        return assignment.Id;
    }

    public async Task EditAssignmentAsync(Guid id, CreateOrEditAssignmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var assignment = await GetAssignmentByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();

        assignment.StatusLog.Add(DateTime.Now, "Обновлено");

        assignment = new(
            dto.AcceptDate,
            dto.DepartmentNumber,
            dto.Description,
            dto.ForwardDate,
            dto.ForwardDeaprtment,
            assignment.StatusLog,
            dto.DepartmentForwardDate,
            dto.Assignee,
            dto.Note,
            assignment.CreationDate,
            dto.Patient
        );

        await _assignmentRepository.UpdateAsync(assignment, cancellationToken);
    }

    public async Task DeleteAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetAssignmentByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();
        await _assignmentRepository.DeleteAsync(assignment, cancellationToken);
    }
}