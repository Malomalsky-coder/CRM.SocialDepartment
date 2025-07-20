using AutoMapper;
using CRM.SocialDepartment.Application.DTOs;
using CRM.SocialDepartment.Domain.Common;
using CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.Data;
using CRM.SocialDepartment.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.SocialDepartment.Application.Users
{
    public class UserAppService(IMapper mapper, UserRepository userRepository)
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserRepository _userRepository = userRepository;

        public async Task<Result> CreateUserAsync(CreateUserDTO input)
        {
            var user = new ApplicationUser();
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.UserName = input.UserName;
            user.Email = input.Email;

            return await _userRepository.CreateAsync(user, input.Password);
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }
    }
}
