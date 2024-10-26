using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserDAO _userDAO;

        public UserRepository(ApplicationDbContext context, UserDAO userDAO)
        {
            _context = context;
            _userDAO = userDAO;

        }

        public async Task<IEnumerable<User>> GetAllUsers() => await _userDAO.GetAllUsers();

        public async Task<IEnumerable<User>> GetAllUsersByRole(int role) => await _userDAO.GetAllUsersByRole(role);

        public async Task<User> GetUserById(int id) => await _userDAO.GetUserById(id);

        public async Task<User> GetUserByEmail(string email) => await _userDAO.GetUserByEmail(email);

        public async Task AddUser(User user) => await _userDAO.AddUser(user);

        public async Task UpdateUser(User user) => await _userDAO.UpdateUser(user);

        public async Task DeleteUser(int id) => await _userDAO.DeleteUser(id);
    }
}
