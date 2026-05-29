namespace MentalHealthSupportAPI.Models
{
    public class CaseNote
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AuthorRole { get; set; } = string.Empty; // Hvem skrev noten?

        public int CaseId { get; set; }
        public Case Case { get; set; } = null!;

        public int AuthorId { get; set; } // Hvilken userId skrev det?
    }
}
