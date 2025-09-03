using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Entities.Patients;
using CRM.SocialDepartment.Domain.Repositories;
using DDD.Entities;
using System.Threading;
using System.Xml.Linq;

namespace CRM.SocialDepartment.Application.Users
{
    public class UserAppService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> CreateUserAsync(CreateUserDTO input)
        {
            try
            {
                // Создаем объект пользователя (тип будет определен в Infrastructure)
                var user = new
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    UserName = input.UserName,
                    Email = input.Email,
                    Role = input.Role,
                    Position = input.Position,
                    DepartmentNumber = input.DepartmentNumber
                };

                // Создаем пользователя
                var createUserResult = await _unitOfWork.Users.CreateAsync(user, input.Password);
                if (!createUserResult.IsSuccess)
                {
                    return createUserResult;
                }

                // Если заполнено поле Role, создаем запись в userRoles
                if (!string.IsNullOrEmpty(input.Role))
                {
                    // Получаем созданного пользователя для получения userId
                    var createdUser = await _unitOfWork.Users.GetAsync((item) => item.UserName == input.UserName);
                    if (createdUser == null)
                    {
                        return Result.Failure("Не удалось получить созданного пользователя");
                    }

                    // Получаем роль по названию для получения roleId
                    var role = await _unitOfWork.Roles.GetAsync((item) => item.Name == input.Role);
                    if (role == null)
                    {
                        return Result.Failure($"Роль '{input.Role}' не найдена");
                    }

                    // Получаем Id пользователя и роли через методы GetId()
                    var userId = createdUser.GetId();
                    var roleId = role.GetId();

                    // Создаем запись в userRoles через новый метод CreateUserIdRoleIdAsync
                    var createUserRoleResult = await _unitOfWork.UserRoles.CreateUserIdRoleIdAsync(
                        roleId, 
                        userId, 
                        CancellationToken.None);

                    if (!createUserRoleResult.IsSuccess)
                    {
                        return createUserRoleResult;
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка создания пользователя: {ex.Message}");
            }
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            var users = _unitOfWork.Users.GetAllUsers();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetUserByNameAsync(string name, CancellationToken cancellationToken = default) 
        {
            var result = await _unitOfWork.Users.GetAsync((item) => item.UserName == name, cancellationToken);
            return result != null ? _mapper.Map<UserDTO>(result) : null;
        }

        public async Task<Result> UpdateUserAsync(CreateUserDTO input)
        {
            // Создаем объект пользователя для обновления
            var user = new
            {
                UserName = input.UserName,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                Role = input.Role,
                Position = input.Position,
                DepartmentNumber = input.DepartmentNumber
            };

            return await _unitOfWork.Users.UpdateAsync(user, input.Password);
        }

        public async Task<Result> DeleteUserAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Users.DeleteAsync(userName, cancellationToken);
        }
    }
}
