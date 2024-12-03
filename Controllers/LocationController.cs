using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;  // For Dapper methods like ExecuteAsync, ExecuteScalarAsync
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public LocationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Endpoint to store location data
    [HttpPost("PushLocation")]
    public async Task<IActionResult> PushLocation([FromBody] LocationModel location)
    {
       
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            try
            {
                await connection.OpenAsync();  // Ensure the connection is open
                var query = "INSERT INTO Locations (StaffID, Latitude, Longitude, Timestamp) VALUES (@StaffID, @Latitude, @Longitude, @Timestamp)";
                var parameters = new
                {
                    location.StaffID,
                    location.Latitude,
                    location.Longitude,
                    location.Timestamp
                };
                await connection.ExecuteAsync(query, parameters); // Dapper ExecuteAsync
            }
            catch { }
        }
            return Ok(new { Status = "Success" });
     
    }

    // Endpoint for user authentication
    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginModel loginModel)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();  // Ensure the connection is open
            var query = "SELECT COUNT(1) FROM Staff WHERE Username = @Username AND Password = @Password";
            var parameters = new
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            };

            var result = await connection.ExecuteScalarAsync<int>(query, parameters); // Dapper ExecuteScalarAsync

            if (result > 0)
            {
                return Ok(new { Status = "Success", Message = "Authentication successful" });
            }
            else
            {
                return Unauthorized(new { Status = "Error", Message = "Invalid username or password" });
            }
        }
    }
}

// Model classes
public class LocationModel
{
    public int StaffID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
