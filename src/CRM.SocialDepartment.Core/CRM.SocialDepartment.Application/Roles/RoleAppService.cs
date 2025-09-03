using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;
using CRM.SocialDepartment.Domain.Entities;

namespace CRM.SocialDepartment.Application.Roles
{
    public class RoleAppService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RoleAppService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> CreateRoleAsync(CreateRoleDTO input)
        {
            try
            {
                // Создаем объект роли (тип будет определен в Infrastructure)
                var role = new
                {
                    Name = input.Name
                };

                // Создаем роль
                var createRoleResult = await _unitOfWork.Roles.CreateAsync(role);
                return createRoleResult;
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка создания роли: {ex.Message}");
            }
        }

        public IEnumerable<RoleDTO> GetAllRoles()
        {
            var roles = _unitOfWork.Roles.GetAllRoles();
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<RoleDTO?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.Roles.GetAsync((item) => item.Name == name, cancellationToken);
            return result != null ? _mapper.Map<RoleDTO>(result) : null;
        }

        public async Task<Result> UpdateRoleAsync(CreateRoleDTO input)
        {
            // Создаем объект роли для обновления
            var role = new
            {
                Name = input.Name
            };

            return await _unitOfWork.Roles.UpdateAsync(role);
        }

        public async Task<Result> DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Roles.DeleteAsync(roleName, cancellationToken);
        }
    }
}
