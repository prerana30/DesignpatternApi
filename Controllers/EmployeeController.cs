using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DesignpatternApi;
using Microsoft.AspNetCore.Mvc;
using Polly;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public EmployeeController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet(Name = "GetEmployeeDetails")]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Define the retry policy
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            // Execute the API call with retry policy
            var result = await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
                response.EnsureSuccessStatusCode();
                var employees = await response.Content.ReadFromJsonAsync<Employee[]>();
                return employees;
            });

            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Failed to fetch employee data: {ex.Message}");
        }
    }
}
