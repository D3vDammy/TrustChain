// Services/Interfaces/ICandidateService.cs
using TrustChain.DTOs.Candidate;

namespace TrustChain.Services.Interfaces;

public interface ICandidateService
{
    Task<List<CandidateDto>> GetByElectionAsync(int electionId);
    Task<CandidateDto> GetByIdAsync(int id);
}