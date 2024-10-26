using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ResponseService
    {
        private readonly ApplicationDbContext _context;

        public ResponseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Response>> GetAllResponses()
        {
            return await _context.Responses.ToListAsync();
        }

        public async Task<Response> GetResponseById(int id)
        {
            return await _context.Responses.FindAsync(id);
        }

        public async Task AddResponse(Response response)
        {
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateResponse(Response response)
        {
            _context.Responses.Update(response);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteResponse(int id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response != null)
            {
                _context.Responses.Remove(response);
                await _context.SaveChangesAsync();
            }
        }
    }
}
