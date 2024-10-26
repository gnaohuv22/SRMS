using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class DepartmentDAO
    {
        private readonly ApplicationDbContext _context;

        public DepartmentDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }
    }
}
