
using Microsoft.EntityFrameworkCore;
using TrustChain.Data;
using TrustChain.DTOs.Candidate;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class CandidateService : ICandidateService
{
    //  All dependencies live here 
    private readonly AppDbContext _db;

    public CandidateService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CandidateDto>> GetByElectionAsync(int electionId)
    {
        var candidates = await _db.Candidates
            .Where(c => c.ElectionId == electionId)
            .Select(c => new CandidateDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Party = c.Party.ToString(),
              
                PartyLogoUrl = c.PartyLogoUrl,
                
                Manifesto = c.Manifesto
            })
            .ToListAsync();

        return candidates;
    }

    public async Task<CandidateDto> GetByIdAsync(int id)
    {
        var candidate = await _db.Candidates
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new Exception("Candidate not found.");

        return new CandidateDto
        {
            Id = candidate.Id,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Party = candidate.Party.ToString(),
          PartyLogoUrl = candidate.PartyLogoUrl,
            
            Manifesto = candidate.Manifesto
        };
    }
}