using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ResponseDAO _responseDAO;

        public ResponseRepository(ApplicationDbContext context, ResponseDAO responseDAO)
        {
            _context = context;
            _responseDAO = responseDAO;
        }

        public async Task<IEnumerable<Response>> GetAllResponses() => await _responseDAO.GetAllResponses();

        public async Task<Response> GetResponseById(int id) => await _responseDAO.GetResponseById(id);

        public async Task AddResponse(Response response) => await _responseDAO.AddResponse(response);

        public async Task UpdateResponse(Response response) => await _responseDAO.UpdateResponse(response);

        public async Task DeleteResponse(int id) => await _responseDAO.DeleteResponse(id);
    }
}
