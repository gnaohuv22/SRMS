using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DepartmentDAO _departmentDAO;

        public DepartmentRepository(ApplicationDbContext context, DepartmentDAO departmentDAO)
        {
            _context = context;
            _departmentDAO = departmentDAO;

        }

        public async Task<IEnumerable<Department>> GetAllDepartments() => await _departmentDAO.GetAllDepartments();

        public async Task<Department> GetDepartmentById(int id) => await _departmentDAO.GetDepartmentById(id);

        public async Task AddDepartment(Department department) => await _departmentDAO.AddDepartment(department);

        public async Task UpdateDepartment(Department department) => await _departmentDAO.UpdateDepartment(department);

        public async Task DeleteDepartment(int id) => await _departmentDAO.DeleteDepartment(id);
    }
}
