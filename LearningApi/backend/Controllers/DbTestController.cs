using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LearningApi.Controllers;

[ApiController]
[Route("api/dbtest")]

public class DbTestController : ControllerBase
{
    private readonly IConfiguration _config;

    public DbTestController(IConfiguration config)
    {
        _config = config;
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

            return Ok("Database connected successfully");
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}