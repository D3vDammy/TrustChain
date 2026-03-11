// Services/Interfaces/IVoteService.cs
using TrustChain.DTOs.Vote;          // ← this was missing

namespace TrustChain.Services.Interfaces;

public interface IVoteService
{
    Task<ElectionResultsDto> CastVoteAsync(int voterId, CastVoteDto dto);
    Task<ElectionResultsDto> GetLiveResultsAsync(int electionId);
}

