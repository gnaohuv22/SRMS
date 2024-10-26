using BusinessObject;
using DataAccess;
using System.Security.Claims;

namespace FormAPI.Services
{
    public interface IAuthentication
    {
        Task<User?> Login(string email);
        Task<bool> Register(User user);
        Task<User?> GetCurrentUser(ClaimsPrincipal user);
    }
    public class Authentication : IAuthentication
    {
        private readonly UserService _userService;

        public Authentication(UserService userService)
        {
            _userService = userService;
        }

        public async Task<User?> Login(string email)
        {
            var entry = await _userService.GetUserByEmail(email);
            return entry;
        }

        public async Task<bool> Register(User user)
        {
            var entry = await _userService.GetUserByEmail(user.Email);
            if (entry == null)
            {
                await _userService.AddUser(user);
                return true;
            }
            return false;
        }

        public async Task<User?> GetCurrentUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int id))
            {
                return await _userService.GetUserById(id);
            }
            return null;
        }
    }
}
