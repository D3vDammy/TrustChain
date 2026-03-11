using TrustChain.DTOs.Candidate;
using TrustChain.DTOs.Election;


namespace TrustChain.Services.Interfaces;

public interface IAdminService
{
    Task<string> CreateElectionAsync(ElectionDto dto);
    Task<string> AddCandidateAsync(CandidateDto dto);
    Task<string> ActivateElectionAsync(int id);
    Task<string> CloseElectionAsync(int id);
    Task<object> GetAllVotersAsync();
    Task<object> GetStatsAsync(int electionId);

    
}