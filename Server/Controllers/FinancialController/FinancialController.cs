using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Soil.Shared.Models.FinancialModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soil.Server.Controllers.FinancialController
{
    [ApiController]
    [Route("api/financial")]
    public class FinancialController : ControllerBase
    {
        private readonly IMongoCollection<FinancialTransaction> _transactions;

        public FinancialController(IConfiguration configuration)
        {
            try
            {
                var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
                var db = client.GetDatabase("DataBase");
                _transactions = db.GetCollection<FinancialTransaction>("FinancialTransactions");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
                throw;
            }
        }

        // GET: api/financial
        [HttpGet]
        public async Task<ActionResult<List<FinancialTransaction>>> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactions.Find(_ => true)
                    .SortByDescending(x => x.TransactionDate)
                    .ToListAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving transactions: {ex.Message}" });
            }
        }

        // GET: api/financial/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialTransaction>> GetTransactionById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid transaction ID format" });

                var transaction = await _transactions.Find(x => x._id == objectId).FirstOrDefaultAsync();
                if (transaction == null)
                    return NotFound(new { message = "Transaction not found" });

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving transaction: {ex.Message}" });
            }
        }

        // POST: api/financial
        [HttpPost]
        public async Task<ActionResult<FinancialTransaction>> CreateTransaction([FromBody] CreateFinancialTransactionDto dto)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(dto.Description))
                    return BadRequest(new { message = "Description is required" });

                if (dto.Amount <= 0)
                    return BadRequest(new { message = "Amount must be greater than 0" });

                if (string.IsNullOrWhiteSpace(dto.TransactionType) || !new[] { "Income", "Expense" }.Contains(dto.TransactionType))
                    return BadRequest(new { message = "Transaction type must be Income or Expense" });

                // Create new transaction
                var transaction = new FinancialTransaction
                {
                    _id = ObjectId.GenerateNewId(),
                    Description = dto.Description,
                    Category = dto.Category ?? "Other",
                    Amount = dto.Amount,
                    TransactionDate = dto.TransactionDate == default ? DateTime.UtcNow : dto.TransactionDate,
                    PaymentMethod = dto.PaymentMethod ?? "Cash",
                    TransactionType = dto.TransactionType,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _transactions.InsertOneAsync(transaction);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error creating transaction: {ex.Message}" });
            }
        }

        // PUT: api/financial/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<FinancialTransaction>> UpdateTransaction(string id, [FromBody] UpdateFinancialTransactionDto dto)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid transaction ID format" });

                if (string.IsNullOrWhiteSpace(dto.Description))
                    return BadRequest(new { message = "Description is required" });

                if (dto.Amount <= 0)
                    return BadRequest(new { message = "Amount must be greater than 0" });

                // Get existing transaction
                var existingTransaction = await _transactions.Find(x => x._id == objectId).FirstOrDefaultAsync();
                if (existingTransaction == null)
                    return NotFound(new { message = "Transaction not found" });

                // Update fields
                existingTransaction.Description = dto.Description;
                existingTransaction.Category = dto.Category ?? "Other";
                existingTransaction.Amount = dto.Amount;
                existingTransaction.TransactionDate = dto.TransactionDate;
                existingTransaction.PaymentMethod = dto.PaymentMethod ?? "Cash";
                existingTransaction.TransactionType = dto.TransactionType;
                existingTransaction.Notes = dto.Notes;
                existingTransaction.UpdatedAt = DateTime.UtcNow;

                var result = await _transactions.ReplaceOneAsync(x => x._id == objectId, existingTransaction);

                if (result.MatchedCount == 0)
                    return NotFound(new { message = "Transaction not found" });

                return Ok(existingTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating transaction: {ex.Message}" });
            }
        }

        // DELETE: api/financial/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid transaction ID format" });

                var result = await _transactions.DeleteOneAsync(x => x._id == objectId);

                if (result.DeletedCount == 0)
                    return NotFound(new { message = "Transaction not found" });

                return Ok(new { message = "Transaction deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting transaction: {ex.Message}" });
            }
        }

        // GET: api/financial/type/{type}
        [HttpGet("type/{type}")]
        public async Task<ActionResult<List<FinancialTransaction>>> GetTransactionsByType(string type)
        {
            try
            {
                if (!new[] { "Income", "Expense" }.Contains(type))
                    return BadRequest(new { message = "Invalid type. Must be Income or Expense" });

                var transactions = await _transactions.Find(x => x.TransactionType == type)
                    .SortByDescending(x => x.TransactionDate)
                    .ToListAsync();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving transactions: {ex.Message}" });
            }
        }

        // GET: api/financial/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<FinancialTransaction>>> GetTransactionsByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    return BadRequest(new { message = "Category is required" });

                var transactions = await _transactions.Find(x => x.Category == category)
                    .SortByDescending(x => x.TransactionDate)
                    .ToListAsync();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving transactions: {ex.Message}" });
            }
        }

        // GET: api/financial/date-range?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet("date-range")]
        public async Task<ActionResult<List<FinancialTransaction>>> GetTransactionsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (endDate < startDate)
                    return BadRequest(new { message = "End date must be greater than or equal to start date" });

                var start = startDate.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(startDate, DateTimeKind.Utc) 
                    : startDate.ToUniversalTime();
                var end = endDate.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc) 
                    : endDate.ToUniversalTime();

                var filter = Builders<FinancialTransaction>.Filter.And(
                    Builders<FinancialTransaction>.Filter.Gte(x => x.TransactionDate, start),
                    Builders<FinancialTransaction>.Filter.Lte(x => x.TransactionDate, end.AddDays(1))
                );

                var transactions = await _transactions.Find(filter)
                    .SortByDescending(x => x.TransactionDate)
                    .ToListAsync();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving transactions: {ex.Message}" });
            }
        }

        // GET: api/financial/summary
        [HttpGet("summary")]
        public async Task<ActionResult<FinancialSummary>> GetFinancialSummary()
        {
            try
            {
                var allTransactions = await _transactions.Find(_ => true).ToListAsync();

                var totalIncome = allTransactions
                    .Where(x => x.TransactionType == "Income")
                    .Sum(x => x.Amount);

                var totalExpenses = allTransactions
                    .Where(x => x.TransactionType == "Expense")
                    .Sum(x => x.Amount);

                var netProfit = totalIncome - totalExpenses;

                var summary = new FinancialSummary
                {
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    NetProfit = netProfit,
                    TotalTransactions = allTransactions.Count,
                    IncomeCount = allTransactions.Count(x => x.TransactionType == "Income"),
                    ExpenseCount = allTransactions.Count(x => x.TransactionType == "Expense"),
                    AverageTransaction = allTransactions.Any() ? allTransactions.Average(x => x.Amount) : 0
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving summary: {ex.Message}" });
            }
        }

        // GET: api/financial/monthly-summary
        [HttpGet("monthly-summary")]
        public async Task<ActionResult<List<MonthlySummary>>> GetMonthlySummary()
        {
            try
            {
                var allTransactions = await _transactions.Find(_ => true).ToListAsync();

                var summary = allTransactions
                    .GroupBy(x => new { x.TransactionDate.Year, x.TransactionDate.Month })
                    .OrderByDescending(g => g.Key.Year)
                    .ThenByDescending(g => g.Key.Month)
                    .Take(12)
                    .Reverse()
                    .Select(g => new MonthlySummary
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        TotalIncome = g.Where(x => x.TransactionType == "Income").Sum(x => x.Amount),
                        TotalExpenses = g.Where(x => x.TransactionType == "Expense").Sum(x => x.Amount),
                        NetProfit = g.Where(x => x.TransactionType == "Income").Sum(x => x.Amount) - 
                                  g.Where(x => x.TransactionType == "Expense").Sum(x => x.Amount)
                    })
                    .ToList();

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving monthly summary: {ex.Message}" });
            }
        }
    }

    // ========== MODELS ==========
    public class FinancialTransaction
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

        [MongoDB.Bson.Serialization.Attributes.BsonElement("description")]
        public string Description { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("category")]
        public string Category { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("amount")]
        public decimal Amount { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("transactionDate")]
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime TransactionDate { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("transactionType")]
        public string TransactionType { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("notes")]
        public string Notes { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("createdAt")]
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("updatedAt")]
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string Id
        {
            get => _id.ToString();
            set
            {
                if (ObjectId.TryParse(value, out var oid))
                {
                    _id = oid;
                }
            }
        }
    }

    public class CreateFinancialTransactionDto
    {
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionType { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateFinancialTransactionDto
    {
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionType { get; set; }
        public string Notes { get; set; }
    }

    public class FinancialSummary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int TotalTransactions { get; set; }
        public int IncomeCount { get; set; }
        public int ExpenseCount { get; set; }
        public decimal AverageTransaction { get; set; }
    }

    public class MonthlySummary
    {
        public string Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
    }
}