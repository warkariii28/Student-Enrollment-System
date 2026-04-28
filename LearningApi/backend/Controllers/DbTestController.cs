using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LearningApi.Helpers;

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

            return Ok(ResponseHelper.Success<object>(null, "Database connected successfully"));
        }
        catch(Exception ex)
        {
            return BadRequest(ResponseHelper.Fail<object>(ex.Message));
        }
    }
}