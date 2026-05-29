namespace MentalHealthSupportAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Aldrig plaintext
        public string Role { get; set; } = string.Empty;         // Admin, Psychologist, User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // En bruger kan have en sag tilknyttet
        public Case? AssignedCase { get; set; }
    }
}
