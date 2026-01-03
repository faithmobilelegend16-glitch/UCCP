using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Soil.Shared.Models.InventoryModel;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Soil.Server.Controllers.InventoryController
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IMongoCollection<UserAccount> _users;
        private readonly IConfiguration _configuration;

        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient(configuration.GetConnectionString("MongoConnection"));
            var db = client.GetDatabase("DataBase");
            _users = db.GetCollection<UserAccount>("users");
        }

        // ================================================
        // ================  SIGN UP  ======================
        // ================================================
        [HttpPost("signup")]
        public async Task<ActionResult<object>> SignUp([FromBody] SignUpDto dto)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(dto?.FullName) || 
                    string.IsNullOrWhiteSpace(dto?.Email) || 
                    string.IsNullOrWhiteSpace(dto?.Password))
                {
                    return BadRequest(new { message = "Full name, email, and password are required." });
                }

                // Check if email already exists
                var existing = await _users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
                if (existing != null)
                    return BadRequest(new { message = "Email already exists." });

                // Create new user
                var user = new UserAccount
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = HashPassword(dto.Password)
                };

                await _users.InsertOneAsync(user);
                return Ok(new { message = "Account created successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Signup Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // ================================================
        // ================  SIGN IN  ======================
        // ================================================
        [HttpPost("signin")]
        public async Task<ActionResult<object>> SignIn([FromBody] SignInDto dto)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(dto?.Email) || 
                    string.IsNullOrWhiteSpace(dto?.Password))
                {
                    return BadRequest(new { message = "Email and password are required." });
                }

                var user = await _users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
                
             

                

                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password." });

                if (!VerifyPassword(dto.Password, user.PasswordHash))
                    return Unauthorized(new { message = "Invalid email or password." });

                return Ok(new { message = "Login successful.", user = new { user.Id, user.FullName, user.Email } });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Signin Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // ================================================
        // ================  HASHING  ======================
        // ================================================
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}