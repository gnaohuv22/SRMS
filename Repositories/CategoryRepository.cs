using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(ApplicationDbContext context, CategoryDAO categoryDAO)
        {
            _context = context;
            _categoryDAO = categoryDAO;

        }

        public async Task<IEnumerable<Category>> GetAllCategories() => await _categoryDAO.GetAllCategories();

        public async Task<Category> GetCategoryById(int id) => await _categoryDAO.GetCategoryById(id);

        public async Task AddCategory(Category category) => await _categoryDAO.AddCategory(category);

        public async Task UpdateCategory(Category category) => await _categoryDAO.UpdateCategory(category);

        public async Task DeleteCategory(int id) => await _categoryDAO.DeleteCategory(id);
    }
}
