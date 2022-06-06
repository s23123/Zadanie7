using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication1.Models.DTO;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public TripsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _dbService.GetTrips();
            return Ok(trips);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTrips(int id)
        {
            await _dbService.RemoveTrip(id);
            return Ok("Removed Trip");
        }

        [HttpPost]
        [Route("{idTrip}/clients")]
        public async Task<IActionResult> AddClientToTrip(SomeSortOfClientTrip clientTrip, int tripId)
        {
            try
            {
                await _dbService.AddClientToTrip(clientTrip, tripId);
                return Ok("Pomyslnie dodano");
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
