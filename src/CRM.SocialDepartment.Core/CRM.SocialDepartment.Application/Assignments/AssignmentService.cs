using System.Globalization;
using System.Linq.Expressions;
using CRM.SocialDepartment.Application.Common;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Application.Patients;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities;
using CRM.SocialDepartment.Domain.Exceptions;
using CRM.SocialDepartment.Domain.Repositories;
using DDD.Events;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.SocialDepartment.Application.Assignments;

public class AssignmentService(IUnitOfWork unitOfWork, IDomainEventDispatcher? domainEventDispatcher = null)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher? _domainEventDispatcher = domainEventDispatcher;

    public async Task<Assignment?> GetAssignmentByIdAsync(Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Assignments.GetAsync(e => e.Id == assignmentId, cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync(Expression<Func<Assignment, bool>>? predicate,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Assignments.GetAllAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Создать новое назначение (с использованием транзакции)
    /// </summary>
    public async Task<Guid> CreateAssignmentAsync(CreateOrEditAssignmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == dto.PatientId, cancellationToken) ??
                      throw new EntityNotFoundException("Пациент не найден");

        var assignment = new Assignment(
            dto.Name,
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
            patient.Id
        );

        await _unitOfWork.Assignments.InsertAsync(assignment, cancellationToken);

        // Публикуем доменные события
        await DomainEventPublisher.PublishAndClearEventsAsync(assignment, _domainEventDispatcher, cancellationToken);

        return assignment.Id;
    }

    /// <summary>
    /// Создать новое назначение с транзакцией (пример)
    /// </summary>
    public async Task<Guid> CreateAssignmentWithTransactionAsync(CreateOrEditAssignmentDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Начинаем транзакцию
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == dto.PatientId, cancellationToken) ??
                          throw new EntityNotFoundException("Пациент не найден");

            var assignment = new Assignment(
                dto.Name,
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
                patient.Id
            );

            // Все операции выполняются в рамках одной транзакции
            await _unitOfWork.Assignments.InsertAsync(assignment, cancellationToken);

            // Публикуем доменные события
            await DomainEventPublisher.PublishAndClearEventsAsync(assignment, _domainEventDispatcher, cancellationToken);

            // Можем также обновить пациента в рамках той же транзакции
            // patient.SomeProperty = "Updated";
            // await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);

            // Подтверждаем транзакцию
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return assignment.Id;
        }
        catch
        {
            // В случае ошибки откатываем транзакцию
            if (_unitOfWork.HasActiveTransaction)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            throw;
        }
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
        if (dto.Note != assignment.Note) assignment.UpdateNote(dto.Note ?? string.Empty);

        await _unitOfWork.Assignments.UpdateAsync(assignment, cancellationToken);

        // Публикуем доменные события
        await DomainEventPublisher.PublishAndClearEventsAsync(assignment, _domainEventDispatcher, cancellationToken);
    }

    public async Task DeleteAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetAssignmentByIdAsync(id, cancellationToken) ?? throw new EntityNotFoundException();
        // Вызываем доменный метод для мягкого удаления
        assignment.SoftDelete();
        
        await _unitOfWork.Assignments.UpdateAsync(assignment, cancellationToken);

        // Публикуем доменные события
        await DomainEventPublisher.PublishAndClearEventsAsync(assignment, _domainEventDispatcher, cancellationToken);
    }

    /// <summary>
    /// Получить активные назначения для DataTables с поддержкой фильтрации и пагинации
    /// </summary>
    public async Task<DataTableResult<Assignment>> GetActiveAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Assignments.GetActiveAssignmentsForDataTableAsync(parameters, cancellationToken);
    }

    /// <summary>
    /// Получить архивные назначения для DataTables с поддержкой фильтрации и пагинации
    /// </summary>
    public async Task<DataTableResult<Assignment>> GetArchivedAssignmentsForDataTableAsync(DataTableParameters parameters, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Assignments.GetArchivedAssignmentsForDataTableAsync(parameters, cancellationToken);
    }
}