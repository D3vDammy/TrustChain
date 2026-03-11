// Models/Vote.cs
namespace TrustChain.Models;

public class Vote
{
    public int Id { get; set; }

    // Who voted
    public int VoterId { get; set; }
    public Voter? Voter { get; set; }

    // Who they voted for
    public int CandidateId { get; set; }
    public Candidate? Candidate { get; set; }    // ← this was missing

    // Which election
    public int ElectionId { get; set; }
    public Election? Election { get; set; }      // ← this was missing

    public DateTime CastAt { get; set; } = DateTime.UtcNow;
}