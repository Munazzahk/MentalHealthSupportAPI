namespace MentalHealthSupportAPI.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string CaseReference { get; set; } = string.Empty; // Fx "CASE-A3F9B2C1" — anonymt
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open"; // Open, Assigned, Closed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Hvilken bruger ejer sagen?
        public int? UserId { get; set; }
        public User? User { get; set; }

        // Hvilken psykolog er tilknyttet?
        public int? PsychologistId { get; set; }
        public Psychologist? Psychologist { get; set; }

        // Noter på sagen
        public ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();
    }
}
