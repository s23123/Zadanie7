using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Services
{
    public class DbService : IDbService
    {
        private readonly APBD7Context _dbContext;
        public DbService(APBD7Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddClientToTrip(SomeSortOfClientTrip clientTrip, int tripId)
        {
            Client client;
            var clientExists = await _dbContext.Clients.AnyAsync(e => e.Pesel == clientTrip.Pesel);

            if (!clientExists)
            {
                 client = new Client() { FirstName = clientTrip.FirstName, LastName = clientTrip.LastName , Email = clientTrip.Email, Telephone = clientTrip.Telephone, Pesel = clientTrip.Pesel};
                await _dbContext.Clients.AddAsync(client);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                 client = await _dbContext.Clients.FirstOrDefaultAsync(e => e.Pesel == clientTrip.Pesel);
            }

            var clientHasTrip = await _dbContext.ClientTrips.AnyAsync(e => e.IdClient == client.IdClient && e.IdClient == tripId);
            if (clientHasTrip)
            {
                throw new System.Exception($"Klient jest juz przypisany do wycieczki o id {tripId}");
            }

            var tripExists = await _dbContext.Trips.AnyAsync(e => e.IdTrip == tripId);
            if (!tripExists)
            {
                throw new System.Exception($"W bazie danych nie ma wycieczki o id {tripId}");
            }

            var tripClient = new ClientTrip() { IdClient = client.IdClient, IdTrip = tripId , RegisteredAt = DateTime.Now, PaymentDate = clientTrip.PaymentDate};

            await _dbContext.ClientTrips.AddAsync(tripClient);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<SomeSortOfTrip>> GetTrips()
        {
            return await _dbContext.Trips.OrderByDescending(e => e.DateFrom)
                .Select(e => new SomeSortOfTrip
                {
                    Name = e.Name,
                    Description = e.Description,
                    MaxPeople = e.MaxPeople,
                    DateFrom = e.DateFrom,
                    DateTo = e.DateTo,
                    Countries = e.CountryTrips.Select(e => new SomeSortOfCountry { Name = e.IdCountryNavigation.Name}).ToList(),
                    Clients = e.ClientTrips.Select(e => new SomeSortOfClient { FirstName = e.IdClientNavigation.FirstName, LastName = e.IdClientNavigation.LastName}).ToList(),
                }).ToListAsync();
        }

        public async Task RemoveClient(int id)
        {

            var hasTrips = await _dbContext.ClientTrips.AnyAsync(e => e.IdClient == id);
            if (hasTrips)
            {
                throw new System.Exception("Klient ma przypisana wycieczke");
            }


            var remove = new Client() { IdClient = id };
                _dbContext.Attach(remove);
                _dbContext.Remove(remove);
                await _dbContext.SaveChangesAsync();
           
            }

        public async Task RemoveTrip(int id)
        {
            


            //dodawanie
            //var addTrip = new Trip { IdTrip = id, Name = "Nazwa" };
            //_dbContext.Add(addTrip);

            ////edycja
            //var editTrip = await _dbContext.Trips.Where(e=> e.IdTrip == id).FirstOrDefaultAsync();
            //editTrip.Name = "aaa";

            //usuwanie
            // var trip = _dbContext.Trips.Where(e => e.IdTrip == id).FirstOrDefaultAsync();
            var trip = new Trip() { IdTrip = id };
            _dbContext.Attach(trip);
            _dbContext.Remove(trip);
            await _dbContext.SaveChangesAsync();
        }
    }
}
