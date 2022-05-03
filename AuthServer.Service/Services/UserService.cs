using AuthServer.Core.Dtos;
using AuthServer.Core.Entities.Concrete;
using AuthServer.Core.Service;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(p => p.Description).ToList();
                return new Response<UserDto>(new ErrorDto(errors, true), 400);
            }

            return new Response<UserDto>(ObjectMapper.Mapper.Map<UserDto>(user), 200);
        }

        public async Task<Response<UserDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return new Response<UserDto>("Username not found.", 404, true);

            return new Response<UserDto>(ObjectMapper.Mapper.Map<UserDto>(user), 200);
        }
    }
}
