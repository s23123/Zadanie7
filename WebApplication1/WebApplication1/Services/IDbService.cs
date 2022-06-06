using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models.DTO;

namespace WebApplication1.Services
{
    public interface IDbService
    {
        Task<IEnumerable<SomeSortOfTrip>> GetTrips();
        Task RemoveTrip(int id);
        Task RemoveClient(int id);
        Task AddClientToTrip(SomeSortOfClientTrip clientTrip, int tripId);
    }
}
