using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Soil.Shared.Models.SaleModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soil.Server.Controllers.SalesController
{
    [ApiController]
    [Route("api/sales")]
    public class SalesController : ControllerBase
    {
        private readonly IMongoCollection<Sale> _sales;

        public SalesController(IConfiguration configuration)
        {
            try
            {
                var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
                var db = client.GetDatabase("DataBase");
                _sales = db.GetCollection<Sale>("Sales");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
                throw;
            }
        }

        // GET: api/sales
        [HttpGet]
        public async Task<ActionResult<List<Sale>>> GetAllSales()
        {
            try
            {
                var sales = await _sales.Find(_ => true)
                    .SortByDescending(x => x.CreatedAt)
                    .ToListAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving sales: {ex.Message}" });
            }
        }

        // GET: api/sales/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSaleById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid sale ID format" });

                var sale = await _sales.Find(s => s._id == objectId).FirstOrDefaultAsync();
                if (sale == null)
                    return NotFound(new { message = "Sale not found" });

                return Ok(sale);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving sale: {ex.Message}" });
            }
        }

        // POST: api/sales
        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale([FromBody] CreateSaleDto dto)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(dto.ProductName))
                    return BadRequest(new { message = "Product name is required" });

                if (dto.Amount <= 0)
                    return BadRequest(new { message = "Amount must be greater than 0" });

                if (dto.Quantity <= 0)
                    return BadRequest(new { message = "Quantity must be greater than 0" });

                // Create new sale
                var sale = new Sale
                {
                    _id = ObjectId.GenerateNewId(),
                    ProductName = dto.ProductName,
                    Quantity = dto.Quantity,
                    Amount = dto.Amount,
                    Category = dto.Category ?? "Other",
                    Description = dto.Description,
                    SaleDate = dto.SaleDate == default ? DateTime.UtcNow : dto.SaleDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _sales.InsertOneAsync(sale);
                return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error creating sale: {ex.Message}" });
            }
        }

        // PUT: api/sales/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Sale>> UpdateSale(string id, [FromBody] UpdateSaleDto dto)
        {
            try
            {
                // Validation
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid sale ID format" });

                if (string.IsNullOrWhiteSpace(dto.ProductName))
                    return BadRequest(new { message = "Product name is required" });

                if (dto.Amount <= 0)
                    return BadRequest(new { message = "Amount must be greater than 0" });

                if (dto.Quantity <= 0)
                    return BadRequest(new { message = "Quantity must be greater than 0" });

                // Get existing sale
                var existingSale = await _sales.Find(s => s._id == objectId).FirstOrDefaultAsync();
                if (existingSale == null)
                    return NotFound(new { message = "Sale not found" });

                // Update fields
                existingSale.ProductName = dto.ProductName;
                existingSale.Quantity = dto.Quantity;
                existingSale.Amount = dto.Amount;
                existingSale.Category = dto.Category ?? "Other";
                existingSale.Description = dto.Description;
                existingSale.SaleDate = dto.SaleDate;
                existingSale.UpdatedAt = DateTime.UtcNow;

                var result = await _sales.ReplaceOneAsync(s => s._id == objectId, existingSale);

                if (result.MatchedCount == 0)
                    return NotFound(new { message = "Sale not found" });

                return Ok(existingSale);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating sale: {ex.Message}" });
            }
        }

        // DELETE: api/sales/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSale(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid sale ID format" });

                var result = await _sales.DeleteOneAsync(s => s._id == objectId);

                if (result.DeletedCount == 0)
                    return NotFound(new { message = "Sale not found" });

                return Ok(new { message = "Sale deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting sale: {ex.Message}" });
            }
        }

        // GET: api/sales/stats/category
        [HttpGet("stats/category")]
        public async Task<ActionResult<List<BsonDocument>>> GetCategoryStats()
        {
            try
            {
                var pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$group", new BsonDocument
                    {
                        { "_id", "$category" },
                        { "totalSales", new BsonDocument("$sum", "$amount") },
                        { "count", new BsonDocument("$sum", 1) }
                    }),
                    new BsonDocument("$sort", new BsonDocument("totalSales", -1))
                };

                var result = await _sales.Aggregate<BsonDocument>(pipeline).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving category stats: {ex.Message}" });
            }
        }

        // GET: api/sales/date-range?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet("date-range")]
        public async Task<ActionResult<List<Sale>>> GetSalesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (endDate < startDate)
                    return BadRequest(new { message = "End date must be greater than or equal to start date" });

                // Convert to UTC if not already
                var start = startDate.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(startDate, DateTimeKind.Utc) 
                    : startDate.ToUniversalTime();
                var end = endDate.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc) 
                    : endDate.ToUniversalTime();

                var filter = Builders<Sale>.Filter.And(
                    Builders<Sale>.Filter.Gte(s => s.SaleDate, start),
                    Builders<Sale>.Filter.Lte(s => s.SaleDate, end.AddDays(1))
                );

                var sales = await _sales.Find(filter)
                    .SortByDescending(x => x.SaleDate)
                    .ToListAsync();

                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving sales by date: {ex.Message}" });
            }
        }

        // GET: api/sales/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<Sale>>> GetSalesByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    return BadRequest(new { message = "Category is required" });

                var sales = await _sales.Find(s => s.Category == category)
                    .SortByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving sales: {ex.Message}" });
            }
        }

        // GET: api/sales/search?query=productName
        [HttpGet("search")]
        public async Task<ActionResult<List<Sale>>> SearchSales([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "Search query is required" });

                var filter = Builders<Sale>.Filter.Or(
                    Builders<Sale>.Filter.Regex(s => s.ProductName, new BsonRegularExpression(query, "i")),
                    Builders<Sale>.Filter.Regex(s => s.Description, new BsonRegularExpression(query, "i"))
                );

                var sales = await _sales.Find(filter)
                    .SortByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error searching sales: {ex.Message}" });
            }
        }
    }
}