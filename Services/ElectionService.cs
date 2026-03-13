// Services/ElectionService.cs
using Microsoft.EntityFrameworkCore;
using TrustChain.Data;
using TrustChain.DTOs.Candidate;
using TrustChain.DTOs.Election;
using TrustChain.DTOs.Vote;
using TrustChain.Enums;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class ElectionService : IElectionService
{

    private readonly AppDbContext _db;

    public ElectionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ElectionDto?> GetActiveElectionAsync()
    {
        var election = await _db.Elections
            .Include(e => e.Candidates)
            .FirstOrDefaultAsync(e => e.Status == ElectionStatus.Active);

        if (election == null) return null;

        return MapToDto(election);
    }

    public async Task<List<ElectionDto>> GetAllElectionsAsync()
    {
        var elections = await _db.Elections
            .Include(e => e.Candidates)
            .ToListAsync();

        return elections.Select(MapToDto).ToList();
    }

    private static ElectionDto MapToDto(Models.Election e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Type = e.Type.ToString(),
        Status = e.Status.ToString(),
        StartTime = e.StartTime,
        EndTime = e.EndTime,
        Candidates = e.Candidates.Select(c => new CandidateDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Party = c.Party.ToString(),
            PartyLogoUrl = c.PartyLogoUrl,
            
            Manifesto = c.Manifesto
        }).ToList()
    };

    public Task CreateElectionAsync(ElectionDto electionDto)
    {
        throw new NotImplementedException();
    }

    public Task AddCandidateAsync(CandidateDto candidateDto)
    {
        throw new NotImplementedException();
    }

    public Task CastVoteAsync(CastVoteDto CastVoteDto)
    {
        throw new NotImplementedException();
    }

    public Task<ElectionResultsDto> GetElectionResultsAsync(int electionId)
    {
        throw new NotImplementedException();
    }
}