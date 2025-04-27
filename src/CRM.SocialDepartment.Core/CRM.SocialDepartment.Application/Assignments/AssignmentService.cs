using System.Globalization;
using System.Linq.Expressions;
using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Exceptions;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CRM.SocialDepartment.Application.Assignments;

public class AssignmentService(
    MongoBasicRepository<Assignment, Guid> assignmentRepository,
    IServiceProvider serviceProvider)
{
    public IMongoCollection<Assignment> GetAssignmentCollection()
    {
        return assignmentRepository.GetCollection();
    }

    public async Task<Assignment?> GetAssignmentByIdAsync(Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        return await assignmentRepository.GetAsync(e => e.Id == assignmentId, cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync(Expression<Func<Assignment, bool>>? predicate,
        CancellationToken cancellationToken = default)
    {
        return await assignmentRepository.GetAllAsync(predicate, cancellationToken);
    }

    public async Task<Guid> CreateAssignmentAsync(CreateOrEditAssignmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var userService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PatientAppService>();
        var patient = await userService.GetPatientByIdAsync(dto.PatientId, cancellationToken) ??
                      throw new EntityNotFoundException("Failed to find patient");

        var assignment = new Assignment(
            dto.AcceptDate,
            dto.DepartmentNumber,
            dto.Description,
            dto.ForwardDate,
            dto.ForwardDepartment,
            new()
            {
                { DateTime.Now.ToString(CultureInfo.InvariantCulture), "Создано" }
            },
            dto.DepartmentForwardDate,
            dto.Assignee,
            dto.Note,
            DateTime.Now,
            patient
        );

        await assignmentRepository.InsertAsync(assignment, cancellationToken);

        return assignment.Id;
    }

    public async Task EditAssignmentAsync(Guid id, CreateOrEditAssignmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var assignment = await GetAssignmentByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();

        assignment.StatusLog.Add(DateTime.Now.ToString(CultureInfo.InvariantCulture), "Обновлено");

        if (!dto.Description.Equals(assignment.Description)) assignment.UpdateDescription(dto.Description);
        if (dto.AcceptDate != assignment.AcceptDate) assignment.UpdateAcceptDate(dto.AcceptDate);
        if (!dto.Assignee.Equals(assignment.Assignee)) assignment.UpdateAssignee(dto.Assignee);
        if (dto.ForwardDate != assignment.ForwardDate) assignment.UpdateForwardDate(dto.ForwardDate);
        if (dto.ForwardDepartment.Equals(assignment.ForwardDepartment))
            assignment.UpdateForwardDepartment(dto.ForwardDepartment);
        if (dto.DepartmentNumber != assignment.DepartmentNumber)
            assignment.UpdateDepartmentNumber(dto.DepartmentNumber);
        if (dto.DepartmentForwardDate != assignment.DepartmentForwardDate)
            assignment.UpdateDepartmentForwardDate(dto.DepartmentForwardDate);
        if (dto.Note != assignment.Note) assignment.UpdateNote(dto.Note);

        await assignmentRepository.UpdateAsync(assignment, cancellationToken);
    }

    public async Task DeleteAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetAssignmentByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();
        await assignmentRepository.DeleteAsync(assignment, cancellationToken);
    }
}