using Microsoft.EntityFrameworkCore;
using MentalHealthSupportAPI.Data;
using MentalHealthSupportAPI.DTOs;
using MentalHealthSupportAPI.Models;

namespace MentalHealthSupportAPI.Services;

public class CaseService : ICaseService
{
    private readonly AppDbContext _context;

    public CaseService(AppDbContext context)
    {
        _context = context;
    }

    // Resource-based authorization — tjekker adgang til specifik sag baseret på rolle + relation
    public async Task<CaseResponse?> GetCaseByIdAsync(int caseId, int requestingUserId, string role)
    {
        var caseEntity = await _context.Cases
            .Include(c => c.Psychologist)
            .Include(c => c.Notes)
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (caseEntity == null) return null;

        // Her sker resource-based authorization
        // Selv om man er autentificeret - har ikke adgang til alt
        bool hasAccess = role switch
        {
            "Admin" => true, // Alt
            "Psychologist" => caseEntity.Psychologist?.UserId == requestingUserId, // Kun egne sager
            "User" => caseEntity.UserId == requestingUserId, // Kun egne sager
            _ => false
        };

        // Return null (404) i stedet for 403 — afslører ikke at sagen eksisterer
        return hasAccess ? MapToResponse(caseEntity) : null;
    }

    public async Task<List<CaseResponse>> GetMyCasesAsync(int userId, string role)
    {
        // Filtrer direkte i databasen — principle of least privilege
        IQueryable<Case> query = _context.Cases
            .Include(c => c.Psychologist)
            .Include(c => c.Notes);

        query = role switch
        {
            "Admin" => query,
            "Psychologist" => query.Where(c => c.Psychologist != null && c.Psychologist.UserId == userId),
            "User" => query.Where(c => c.UserId == userId),
            _ => query.Where(_ => false)
        };

        return (await query.ToListAsync()).Select(MapToResponse).ToList();
    }

    public async Task<CaseResponse> CreateCaseAsync(CreateCaseRequest request, int userId)
    {
        var caseEntity = new Case
        {
            CaseReference = GenerateCaseReference(), // Anonym referencenummer
            Description = request.Description,
            Status = "Open",
            UserId = userId
        };

        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();

        return MapToResponse(caseEntity);
    }

    // Bruges i AdminController - kun admin kan tildele sager
    public async Task<bool> AssignCaseAsync(AssignCaseRequest request)
    {
        var caseEntity = await _context.Cases.FindAsync(request.CaseId);
        var psychologist = await _context.Psychologists.FindAsync(request.PsychologistId);

        if (caseEntity == null || psychologist == null) return false;

        caseEntity.PsychologistId = request.PsychologistId;
        caseEntity.UserId = request.UserId;
        caseEntity.Status = "Assigned";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CaseNoteResponse?> AddNoteAsync(int caseId, AddNoteRequest request, int authorId, string authorRole)
    {
        var caseEntity = await _context.Cases
            .Include(c => c.Psychologist)
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (caseEntity == null) return null;

        // Adgangskontrol — kun relevante person kan tilføje noter
        bool hasAccess = authorRole switch
        {
            "Admin" => true,
            "Psychologist" => caseEntity.Psychologist?.UserId == authorId,
            "User" => caseEntity.UserId == authorId,
            _ => false
        };

        if (!hasAccess) return null;

        var note = new CaseNote
        {
            CaseId = caseId,
            Content = request.Content,
            AuthorId = authorId,
            AuthorRole = authorRole
        };

        _context.CaseNotes.Add(note);
        await _context.SaveChangesAsync();

        return new CaseNoteResponse
        {
            Id = note.Id,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            AuthorRole = note.AuthorRole
        };
    }

    // Genererer anonymt referencenummer
    private static string GenerateCaseReference()
        => $"CASE-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

    // Mapper Case entitet til CaseResponse DTO - skjuler følsomme data og formaterer output
    private static CaseResponse MapToResponse(Case c) => new()
    {
        Id = c.Id,
        CaseReference = c.CaseReference,
        Description = c.Description,
        Status = c.Status,
        PsychologistName = c.Psychologist?.DisplayName,
        Notes = c.Notes.Select(n => new CaseNoteResponse
        {
            Id = n.Id,
            Content = n.Content,
            CreatedAt = n.CreatedAt,
            AuthorRole = n.AuthorRole
        }).ToList()
    };
}