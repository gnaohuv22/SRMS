using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartments();
        Task<Department> GetDepartmentById(int id);
        Task AddDepartment(Department department);
        Task UpdateDepartment(Department department);
        Task DeleteDepartment(int id);
    }
}
