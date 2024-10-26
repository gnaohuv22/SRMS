using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IResponseRepository
    {
        Task<IEnumerable<Response>> GetAllResponses();
        Task<Response> GetResponseById(int id);
        Task AddResponse(Response response);
        Task UpdateResponse(Response response);
        Task DeleteResponse(int id);
    }
}
