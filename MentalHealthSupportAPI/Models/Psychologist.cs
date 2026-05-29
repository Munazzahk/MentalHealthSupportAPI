namespace MentalHealthSupportAPI.Models
{
    public class Psychologist
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;

        // Psykologen er koblet til user (login-identitet)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Psykolog kan have mange sager
        public ICollection<Case> Cases { get; set; } = new List<Case>();
    }
}
