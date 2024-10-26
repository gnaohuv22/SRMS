using BusinessObject;
using Repositories;
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
        private readonly IUserRepository _userRepository;

        public Authentication(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> Login(string email)
        {
            var entry = await _userRepository.GetUserByEmail(email);
            return entry;
        }

        public async Task<bool> Register(User user)
        {
            var entry = await _userRepository.GetUserByEmail(user.Email);
            if (entry == null)
            {
                await _userRepository.AddUser(user);
                return true;
            }
            return false;
        }

        public async Task<User?> GetCurrentUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int id))
            {
                return await _userRepository.GetUserById(id);
            }
            return null;
        }
    }
}
