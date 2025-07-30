using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Domain.Repositories;

namespace CRM.SocialDepartment.Application.Users
{
    public class UserAppService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> CreateUserAsync(CreateUserDTO input)
        {
            // Создаем объект пользователя (тип будет определен в Infrastructure)
            var user = new
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                UserName = input.UserName,
                Email = input.Email
            };

            return await _unitOfWork.Users.CreateAsync(user, input.Password);
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            var users = _unitOfWork.Users.GetAllUsers();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }
    }
}
