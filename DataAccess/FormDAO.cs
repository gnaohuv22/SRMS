using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class FormDAO
    {
        private readonly ApplicationDbContext _context;

        public FormDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Form>> GetAllForms()
        {
            return await _context.Forms
                .Include(f => f.User)
                .Include(f => f.Category)
                .ToListAsync();
        }

        public async Task<Form> GetFormById(int id)
        {
            return await _context.Forms
                .Include(f => f.User)
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.FormId == id);
        }

        public async Task AddForm(Form form)
        {
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateForm(Form form)
        {
            _context.Forms.Update(form);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteForm(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                _context.Forms.Remove(form);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Form>> GetFormsByStudentId(int studentId)
        {
            return await _context.Forms
                .Include(f => f.User)
                .Include(f => f.Category)
                .Where(f => f.StudentId == studentId)
                .ToListAsync();
        }
    }
}
