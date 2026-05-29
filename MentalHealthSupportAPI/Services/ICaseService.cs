using MentalHealthSupportAPI.DTOs;

namespace MentalHealthSupportAPI.Services
{
    public interface ICaseService
    {
        Task<CaseResponse?> GetCaseByIdAsync(int caseId, int requestingUserId, string role);
        Task<List<CaseResponse>> GetMyCasesAsync(int userId, string role);
        Task<CaseResponse> CreateCaseAsync(CreateCaseRequest request, int userId);
        Task<bool> AssignCaseAsync(AssignCaseRequest request);
        Task<CaseNoteResponse?> AddNoteAsync(int caseId, AddNoteRequest request, int authorId, string authorRole);
    }
}
