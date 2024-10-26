using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly FormDAO _formDAO;

        public FormRepository(ApplicationDbContext context, FormDAO formDAO)
        {
            _context = context;
            _formDAO = formDAO;

        }

        public async Task<IEnumerable<Form>> GetAllForms() => await _formDAO.GetAllForms();

        public async Task<Form> GetFormById(int id) => await _formDAO.GetFormById(id);

        public async Task AddForm(Form form) => await _formDAO.AddForm(form);

        public async Task UpdateForm(Form form) => await _formDAO.UpdateForm(form);

        public async Task DeleteForm(int id) => await _formDAO.DeleteForm(id);

        public async Task<IEnumerable<Form>> GetFormsByStudentId(int studentId) => await _formDAO.GetFormsByStudentId(studentId);
    }
}
