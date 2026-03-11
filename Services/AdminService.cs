using Microsoft.EntityFrameworkCore;
using TrustChain.Data;
using TrustChain.DTOs.Candidate;
using TrustChain.DTOs.Election;
using TrustChain.Enums;
using TrustChain.Models;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;

    public AdminService(AppDbContext db)
    {
        _db = db;
    }

    // Create Election
    public async Task<string> CreateElectionAsync(ElectionDto dto)
    {
        var election = new Election
        {
            Title = dto.Title,
            Type = Enum.Parse<ElectionType>(dto.Type),
            Status = ElectionStatus.Upcoming,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        _db.Elections.Add(election);
        await _db.SaveChangesAsync();

        return "Election created successfully.";
    }

    // Add Candidate 
    public async Task<string> AddCandidateAsync(CandidateDto dto)
    {
        var election = await _db.Elections.FindAsync(dto.Id)
            ?? throw new Exception("Election not found.");

        var candidate = new Candidate
        {
             FirstName = dto.FirstName ?? string.Empty,              
             Party = Enum.Parse<PoliticalParty>(dto.Party ?? "Other"),
             PartyLogoUrl = dto.PartyLogoUrl ?? string.Empty,
             StateOfOrigin = dto.StateOfOrigin ?? string.Empty,    
             Manifesto = dto.Manifesto ?? string.Empty,         
             ElectionId = election.Id,
        };

        _db.Candidates.Add(candidate);
        await _db.SaveChangesAsync();

        return "Candidate added successfully.";
    }

    // Activate Election 
    public async Task<string> ActivateElectionAsync(int id)
    {
        var election = await _db.Elections.FindAsync(id)
            ?? throw new Exception("Election not found.");

        election.Status = ElectionStatus.Active;
        await _db.SaveChangesAsync();

        return "Election is now active.";
    }

    //  Close Election 
    public async Task<string> CloseElectionAsync(int id)
    {
        var election = await _db.Elections.FindAsync(id)
            ?? throw new Exception("Election not found.");

        election.Status = ElectionStatus.Completed;
        await _db.SaveChangesAsync();

        return "Election has been closed.";
    }

    //  Get All Voters 
    public async Task<object> GetAllVotersAsync()
    {
        var voters = await _db.Voters
          
            .Select(v => new
            {
                v.Id,
                v.FirstName,
                v.LastName,
                v.NIN,
                v.PhoneNumber,
                v.StateOfOrigin,
                v.HasVoted,
                v.IsVerified,
               
            })
            .ToListAsync();

        return voters;
    }

    // Get Stats 
    public async Task<object> GetStatsAsync(int electionId)
    {
        var totalVoters = await _db.Voters.CountAsync();
        var totalVoted = await _db.Votes
            .Where(v => v.ElectionId == electionId)
            .Select(v => v.VoterId)
            .Distinct()
            .CountAsync();

        return new
        {
            TotalRegisteredVoters = totalVoters,
            TotalVotesCast = totalVoted,
            Turnout = totalVoters == 0 ? 0 :
                Math.Round((double)totalVoted / totalVoters * 100, 1)
        };
    }

   
}