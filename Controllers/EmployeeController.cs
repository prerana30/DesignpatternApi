using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DesignpatternApi;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly HttpClient _httpClient;
    public EmployeeController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    [HttpGet(Name = "GetEmpoyeeDetails")]
    public async Task<IActionResult> Get()
    {
        int maxRetryAttempts = 3;
        TimeSpan retryInterval = TimeSpan.FromSeconds(1);
        int retryCount = 0;
        do
        {
            try
            {
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/usering");
                response.EnsureSuccessStatusCode();
                var students = await response.Content.ReadFromJsonAsync<Employee[]>();
                return Ok(students);
            }
            catch (HttpRequestException)
            {
                if (retryCount < maxRetryAttempts)
                {
                    await Task.Delay(retryInterval);
                    retryCount++;
                }
                else
                {
                    return StatusCode(500, "Failed to fetch employee data after multiple attempts.");
                }
            }
        } while (retryCount < maxRetryAttempts);
        return StatusCode(500, "Failed to fetch employee data after multiple attempts.");
    }
}