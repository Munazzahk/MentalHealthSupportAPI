namespace MentalHealthSupportAPI.DTOs
{
    public class CreateCaseRequest
    {
        public string Description { get; set; } = string.Empty;
    }

    public class AssignCaseRequest
    {
        public int CaseId { get; set; }
        public int PsychologistId { get; set; }
        public int UserId { get; set; }
    }

    public class AddNoteRequest
    {
        public string Content { get; set; } = string.Empty;
    }

    public class CaseResponse
    {
        public int Id { get; set; }
        public string CaseReference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? PsychologistName { get; set; }
        public List<CaseNoteResponse> Notes { get; set; } = new();
    }

    public class CaseNoteResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorRole { get; set; } = string.Empty; // Rolle og ikke navn — anonymitet
    }
}
