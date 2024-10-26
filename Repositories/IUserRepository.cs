using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<IEnumerable<User>> GetAllUsersByRole(int role);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
    }
}
