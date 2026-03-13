
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TrustChain.Data;
using TrustChain.DTOs.Vote;
using TrustChain.Enums;
using TrustChain.Hubs;
using TrustChain.Models;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class VoteService : IVoteService
{
    
    private readonly AppDbContext _db;
    private readonly IHubContext<VoteHub> _hubContext;

    public VoteService(AppDbContext db, IHubContext<VoteHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    // PAGE 5: Cast Vote
    public async Task<ElectionResultsDto> CastVoteAsync(int voterId, CastVoteDto dto)
    {
        var voter = await _db.Voters.FindAsync(voterId)
            ?? throw new Exception("Voter not found.");

        if (voter.HasVoted)
            throw new Exception("You have already cast your vote.");

        var alreadyVoted = await _db.Votes
            .AnyAsync(v => v.VoterId == voterId && v.ElectionId == dto.ElectionId);
        if (alreadyVoted)
            throw new Exception("Duplicate vote detected.");

        var election = await _db.Elections.FindAsync(dto.ElectionId)
            ?? throw new Exception("Election not found.");

        if (election.Status != ElectionStatus.Active)
            throw new Exception("This election is not currently active.");

        if (DateTime.UtcNow < election.StartTime)
            throw new Exception("Voting has not started yet.");

        if (DateTime.UtcNow > election.EndTime)
            throw new Exception("Voting has closed.");

        var candidate = await _db.Candidates
            .FirstOrDefaultAsync(c => c.Id == dto.CandidateId
                               && c.ElectionId == dto.ElectionId)
            ?? throw new Exception("Invalid candidate for this election.");

        _db.Votes.Add(new Vote
        {
            VoterId = voterId,
            CandidateId = dto.CandidateId,
            ElectionId = dto.ElectionId,
            CastAt = DateTime.UtcNow
        });

        voter.HasVoted = true;
        await _db.SaveChangesAsync();

        var results = await GetLiveResultsAsync(dto.ElectionId);

        // Broadcast to ALL clients via Signal
        await _hubContext.Clients.All.SendAsync("ReceiveResults", results);

        return results;
    }

    //  Get Live Results 
    public async Task<ElectionResultsDto> GetLiveResultsAsync(int electionId)
    {
        var election = await _db.Elections
            .Include(e => e.Candidates)
            .FirstOrDefaultAsync(e => e.Id == electionId)
            ?? throw new Exception("Election not found.");

        var totalVotes = await _db.Votes
            .CountAsync(v => v.ElectionId == electionId);

        var results = new List<VoteResultDto>();

        foreach (var candidate in election.Candidates)
        {
            var candidateVotes = await _db.Votes
                .CountAsync(v => v.CandidateId == candidate.Id
                            && v.ElectionId == electionId);

            results.Add(new VoteResultDto
            {
                CandidateId = candidate.Id,
                CandidateName = candidate.FirstName + " " + candidate.LastName,
                Party = candidate.Party.ToString(),
                PhotoUrl = candidate.PhotoUrl,
                PartyLogoUrl = candidate.PartyLogoUrl,
               
                TotalVotes = candidateVotes,
                Percentage = totalVotes == 0 ? 0 :
                    Math.Round((double)candidateVotes / totalVotes * 100, 1)
            });
        }

        if (results.Any())
        {
            var maxVotes = results.Max(r => r.TotalVotes);
            if (maxVotes > 0)
                foreach (var r in results.Where(r => r.TotalVotes == maxVotes))
                    r.IsLeading = true;
        }

        return new ElectionResultsDto
        {
            ElectionId = election.Id,
            ElectionTitle = election.Title,
            TotalVotesCast = totalVotes,
            Status = election.Status.ToString(),
            Results = results.OrderByDescending(r => r.TotalVotes).ToList(),
            LastUpdated = DateTime.UtcNow
        };
    }
}