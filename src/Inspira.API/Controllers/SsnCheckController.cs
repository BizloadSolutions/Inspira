using Microsoft.AspNetCore.Mvc;
using Inspira.API.Models;
using Inspira.Application.Services;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace Inspira.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SsnScope")]
public class SsnCheckController : ControllerBase
{
    private readonly ISsnCheckService _ssnCheckService;
    private readonly IMapper _mapper;

    public SsnCheckController(ISsnCheckService ssnCheckService, IMapper mapper)
    {
        _ssnCheckService = ssnCheckService;
        _mapper = mapper;
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
        
        // Validate Sanitisation for SSN
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (string.IsNullOrWhiteSpace(request.SSN))
            return BadRequest(new { error = "SSN is required." });

        if (string.IsNullOrWhiteSpace(request.Role))
            return BadRequest(new { error = "Role is required." });

        try
        {
            var serviceResult = await _ssnCheckService.SsnCheckAsync(submissionId, request.SSN, request.Role);
            var responseDto = _mapper.Map<SsnCheckResponseDto>(serviceResult);

            return Ok(responseDto);
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
