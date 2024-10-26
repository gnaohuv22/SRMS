using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IFormRepository
    {
        Task<IEnumerable<Form>> GetAllForms();
        Task<Form> GetFormById(int id);
        Task AddForm(Form form);
        Task UpdateForm(Form form);
        Task DeleteForm(int id);
        Task<IEnumerable<Form>> GetFormsByStudentId(int studentId);
    }
}
