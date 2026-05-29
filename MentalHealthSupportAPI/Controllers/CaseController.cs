using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MentalHealthSupportAPI.DTOs;
using MentalHealthSupportAPI.Services;

namespace MentalHealthSupportAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Alle endpoints kræver login
public class CaseController : ControllerBase
{
    private readonly ICaseService _caseService;

    public CaseController(ICaseService caseService)
        => _caseService = caseService;

    // User Id + rolle - direkte fra token og ikke fra request body (undgår spoofing)
    private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetRole()
        => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCase(int id)
    {
        var result = await _caseService.GetCaseByIdAsync(id, GetUserId(), GetRole());
        // 404 i stedet for 403 — afslører ikke at case findes (privacy by design)
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("my-cases")]
    public async Task<IActionResult> GetMyCases()
    {
        var result = await _caseService.GetMyCasesAsync(GetUserId(), GetRole());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "User")] // Kun User kan oprette sager (ikke psykologer eller admins)
    public async Task<IActionResult> CreateCase(CreateCaseRequest request)
    {
        var result = await _caseService.CreateCaseAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetCase), new { id = result.Id }, result);
    }

    // Service tjekker for adgang
    [HttpPost("{id}/notes")]
    public async Task<IActionResult> AddNote(int id, AddNoteRequest request)
    {
        var result = await _caseService.AddNoteAsync(id, request, GetUserId(), GetRole());
        if (result == null) return NotFound();
        return Ok(result);
    }
}