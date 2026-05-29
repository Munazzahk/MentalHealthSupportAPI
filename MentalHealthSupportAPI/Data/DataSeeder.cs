using MentalHealthSupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthSupportAPI.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Kør kun hvis database tom
        if (await context.Users.AnyAsync()) return;

        // Users
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        };

        var psychUser = new User
        {
            Username = "psych1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Psych123!"),
            Role = "Psychologist"
        };

        var user1 = new User
        {
            Username = "anon_user_1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            Role = "User"
        };

        var user2 = new User
        {
            Username = "anon_user_2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            Role = "User"
        };

        context.Users.AddRange(adminUser, psychUser, user1, user2);
        await context.SaveChangesAsync();

        // Psykolog profil 
        var psychologist = new Psychologist
        {
            DisplayName = "Dr. Jensen",
            UserId = psychUser.Id
        };

        context.Psychologists.Add(psychologist);
        await context.SaveChangesAsync();

        // Sager 
        var case1 = new Case
        {
            CaseReference = "CASE-AA11BB22",
            Description = "Oplever angst og søvnproblemer.",
            Status = "Assigned",
            UserId = user1.Id,
            PsychologistId = psychologist.Id
        };

        var case2 = new Case
        {
            CaseReference = "CASE-CC33DD44",
            Description = "Har brug for støtte efter svær periode.",
            Status = "Open",
            UserId = user2.Id
        };

        context.Cases.AddRange(case1, case2);
        await context.SaveChangesAsync();

        // Noter - sag 1 
        context.CaseNotes.AddRange(
            new CaseNote
            {
                CaseId = case1.Id,
                Content = "Første samtale gennemført. Brugeren virker åben.",
                AuthorId = psychUser.Id,
                AuthorRole = "Psychologist"
            },
            new CaseNote
            {
                CaseId = case1.Id,
                Content = "Jeg føler mig lidt bedre efter samtalen.",
                AuthorId = user1.Id,
                AuthorRole = "User"
            }
        );

        await context.SaveChangesAsync();
    }
}