using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LearningApi.Helpers;
using Microsoft.AspNetCore.Authorization;
namespace LearningApi.Controllers;

[ApiController]
[Route("api/dbtest")]
[Authorize(Roles = "Admin")]

public class DbTestController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<DbTestController> _logger;

    public DbTestController(IConfiguration config,ILogger<DbTestController> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult TestConnection()
    {
        string connString =
            _config.GetConnectionString("DefaultConnection");

        using SqlConnection conn =
            new SqlConnection(connString);

        try
        {
            conn.Open();

            return Ok(ResponseHelper.Success<object>(null, "Database connected successfully"));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return BadRequest(ResponseHelper.Fail<object>(ex.Message));
        }
    }

    
}