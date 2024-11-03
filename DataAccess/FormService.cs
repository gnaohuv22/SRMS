using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class FormService
    {
        private readonly ApplicationDbContext _context;

        public FormService(ApplicationDbContext context)
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

        public async Task<IEnumerable<Form>> GetFormsByUserDepartmentId(int departmentId)
        {
            return await _context.Forms
                .Include(f => f.User)
                .Include(f => f.Category)
                .Where(f => f.Category.DepartmentUserId == departmentId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Form>> GetFormsByUserId(int userId)
        {
            return await _context.Forms
                .Include(f => f.User)
                .Include(f => f.Category)
                .Where(f => f.User.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        // Get forms filtered by status for a department
        public async Task<IEnumerable<Form>> GetDepartmentFormsByStatusAsync(int departmentUserId, FormStatus status)
        {
            return await _context.Forms
                .Include(f => f.Category)
                .Include(f => f.User)
                .Where(f => f.Category.DepartmentUserId == departmentUserId && f.Status == status)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        // Get forms for a specific category
        public async Task<IEnumerable<Form>> GetFormsByCategoryAsync(int categoryId)
        {
            return await _context.Forms
                .Include(f => f.Category)
                .Include(f => f.User)
                .Where(f => f.CategoryId == categoryId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        // Get all categories managed by a department user
        public async Task<IEnumerable<Category>> GetDepartmentCategoriesAsync(int departmentUserId)
        {
            return await _context.Categories
                .Where(c => c.DepartmentUserId == departmentUserId)
                .ToListAsync();
        }

        // Get forms with responses
        public async Task<IEnumerable<Form>> GetDepartmentFormsWithResponsesAsync(int departmentUserId)
        {
            return await _context.Forms
                .Include(f => f.Category)
                .Include(f => f.User)
                .Include(f => f.Responses)
                    .ThenInclude(r => r.User)
                .Where(f => f.Category.DepartmentUserId == departmentUserId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        // Get form statistics for a department
        public async Task<DepartmentFormStatistics> GetDepartmentFormStatisticsAsync(int departmentUserId)
        {
            var forms = await _context.Forms
                .Where(f => f.Category.DepartmentUserId == departmentUserId)
                .ToListAsync();

            return new DepartmentFormStatistics
            {
                TotalForms = forms.Count,
                PendingForms = forms.Count(f => f.Status == FormStatus.Pending),
                ProcessingForms = forms.Count(f => f.Status == FormStatus.Processing),
                AcceptedForms = forms.Count(f => f.Status == FormStatus.Accepted),
                RejectedForms = forms.Count(f => f.Status == FormStatus.Rejected)
            };
        }

        public async Task<Response> GetResponseByFormId(int formId)
        {
            var response = await _context.Responses
                .Where(r => r.FormId == formId)
                .FirstOrDefaultAsync();
            return response;
        }
    }

    public class DepartmentFormStatistics
    {
        public int TotalForms { get; set; }
        public int PendingForms { get; set; }
        public int ProcessingForms { get; set; }
        public int AcceptedForms { get; set; }
        public int RejectedForms { get; set; }
    }
}
