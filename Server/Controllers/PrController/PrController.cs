using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Soil.Shared.Models.PurchaseRequests;

namespace Soil.Server.Controllers.PrController
{
    [ApiController]
    [Route("api/purchaserequest")]
    public class PrController : ControllerBase
    {
        private readonly IMongoCollection<PrClass> _prs;

        public PrController(IConfiguration configuration)
        {
            try
            {
                var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
                var db = client.GetDatabase("DataBase");
                _prs = db.GetCollection<PrClass>("purchases");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
                throw;
            }
        }

        // GET: api/purchaserequest
        [HttpGet]
        public async Task<ActionResult<List<PrClass>>> GetAllPrs()
        {
            try
            {
                var prs = await _prs.Find(_ => true)
                                    .SortByDescending(x => x.RequestDate)
                                    .ToListAsync();
                return Ok(prs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving PRs: {ex.Message}" });
            }
        }

        // GET: api/purchaserequest/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PrClass>> GetPrById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                    return BadRequest(new { message = "Invalid PR ID format" });

                var pr = await _prs.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (pr == null) return NotFound(new { message = "PR not found" });

                return Ok(pr);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving PR: {ex.Message}" });
            }
        }

        // POST: api/purchaserequest/savepr
        [HttpPost("savepr")]
        public async Task<ActionResult<PrClass>> SavePr([FromBody] PrClass pr)
        {
            if (pr == null)
                return BadRequest(new { message = "PR data is null" });

            try
            {
                pr.RequestDate = pr.RequestDate == default ? DateTime.UtcNow : pr.RequestDate;

                if (string.IsNullOrEmpty(pr.Id))
                {
                    // CREATE
                    pr.Id = ObjectId.GenerateNewId().ToString();
                    await _prs.InsertOneAsync(pr);
                    return Ok(pr);
                }
                else
                {
                    // UPDATE
                    var existingPr = await _prs.Find(p => p.Id == pr.Id).FirstOrDefaultAsync();
                    if (existingPr == null) return NotFound(new { message = "PR not found" });

                    await _prs.ReplaceOneAsync(p => p.Id == pr.Id, pr);
                    return Ok(pr);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving PR: {ex.Message}" });
            }
        }

        // DELETE: api/purchaserequest/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePr(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest(new { message = "Invalid PR ID format" });

            try
            {
                var result = await _prs.DeleteOneAsync(p => p.Id == id);
                if (result.DeletedCount == 0)
                    return NotFound(new { message = "PR not found" });

                return Ok(new { message = "PR deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting PR: {ex.Message}" });
            }
        }
    }
}
