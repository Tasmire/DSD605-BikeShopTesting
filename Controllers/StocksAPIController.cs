using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DSD603_BikeShopDB.Data;
using DSD603_Bike_Shop.Models;

namespace DSD603_BikeShopDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StocksAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/StocksAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStock()
        {
            return await _context.Stock.ToListAsync();
        }

        // GET: api/StocksAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStock(Guid id)
        {
            var stock = await _context.Stock.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return stock;
        }

        // PUT: api/StocksAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(Guid id, Stock stock)
        {
            if (id != stock.StockId)
            {
                return BadRequest();
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
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

        // POST: api/StocksAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(Stock stock)
        {
            _context.Stock.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStock", new { id = stock.StockId }, stock);
        }

        // DELETE: api/StocksAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(Guid id)
        {
            var stock = await _context.Stock.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stock.Remove(stock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StockExists(Guid id)
        {
            return _context.Stock.Any(e => e.StockId == id);
        }
    }
}
