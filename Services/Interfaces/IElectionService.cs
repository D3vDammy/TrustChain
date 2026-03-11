using TrustChain.DTOs.Candidate;
using TrustChain.DTOs.Election;
using TrustChain.DTOs.Vote;

namespace TrustChain.Services.Interfaces;

public interface IElectionService
{
    Task<ElectionDto?> GetActiveElectionAsync();
    Task<List<ElectionDto>> GetAllElectionsAsync();
    Task CreateElectionAsync(ElectionDto electionDto);
    Task AddCandidateAsync(CandidateDto candidateDto);
    Task CastVoteAsync(CastVoteDto CastVoteDto);
    Task<ElectionResultsDto> GetElectionResultsAsync(int electionId);
}