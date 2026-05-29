using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MentalHealthSupportAPI.DTOs;
using MentalHealthSupportAPI.Services;

namespace MentalHealthSupportAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Hele controller kun for Admin
public class AdminController : ControllerBase
{
    private readonly ICaseService _caseService;

    public AdminController(ICaseService caseService)
        => _caseService = caseService;

    [HttpPost("assign-case")]
    public async Task<IActionResult> AssignCase(AssignCaseRequest request)
    {
        var success = await _caseService.AssignCaseAsync(request);
        if (!success) return BadRequest(new { message = "Could not assign case." });
        return Ok(new { message = "Case assigned." });
    }
}