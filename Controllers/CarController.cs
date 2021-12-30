using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApi.Models;

namespace webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarContext _context;

        public CarController(CarContext context)
        {
            _context = context;
        }

        //POST: api/car/init/{number}
        [Route("/api/Car/init/{number}")][HttpGet]
        public async Task<bool> Init(int number){
            if(number< 1) return false;
            if(number > 10) return false;

            var rand = new Random();

            for(int i = 0; i < number; i++){
                DateTime RandomDate = DateTime.Today.AddYears(-rand.Next(5, 100));
                _context.Cars.Add(new Car(){ Year = RandomDate, Make = $"Car Make {i}", Model = $"Car Model {i}", Passengers = rand.Next(5)});
            }
            await _context.SaveChangesAsync();
            return true;
        }

        // GET: api/Car
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            
            return await _context.Cars.ToListAsync();
        }

        // GET: api/Car/Passengers/4
        [Route("/api/Car/Passengers/{num}")][HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarPass(int num)
        {
            
            var passengers = await _context.Cars.Where(c => c.Passengers <= num)
            .OrderByDescending(c => c.Year )
            .ToListAsync();

            return passengers;

            

        }

        // GET: api/Car/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(long id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // PUT: api/Car/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(long id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Car
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        // DELETE: api/Car/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(long id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(long id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
