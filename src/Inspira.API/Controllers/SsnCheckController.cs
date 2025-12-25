using Microsoft.AspNetCore.Mvc;
using Inspira.API.Models;
using Inspira.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Inspira.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SsnScope")]
public class SsnCheckController : ControllerBase
{
    private readonly ISsnCheckService _ssnCheckService;

    public SsnCheckController(ISsnCheckService ssnCheckService)
    {
        _ssnCheckService = ssnCheckService;
    }

    /// <summary>
    /// Accepts SSN and Role, validates input, and performs business logic.
    /// Returns:
    /// - 200 OK with result when successful
    /// - 400 Bad Request for validation errors
    /// - 404 Not Found when related resources are missing
    /// - 500 Internal Server Error for unexpected failures
    /// </summary>
    [HttpPost("{submissionId:int}")]
    public async Task<IActionResult> Post(int? submissionId, [FromBody] SsnCheckRequestDto request)
    {
        if (request is null || submissionId is null)
            return BadRequest("Request is missing required parameters.");

        if (string.IsNullOrWhiteSpace(request.SSN))
            return BadRequest(new { error = "SSN is required." });

        if (string.IsNullOrWhiteSpace(request.Role))
            return BadRequest(new { error = "Role is required." });

        try
        {
            var json = await _ssnCheckService.SsnCheckAsync(submissionId, request.SSN, request.Role);
            return Content(json, "application/json");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (System.Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }
}
