// Models/Election.cs
using TrustChain.Enums;

namespace TrustChain.Models;

public class Election
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ElectionType Type { get; set; }           
    public ElectionStatus Status { get; set; } = ElectionStatus.Upcoming;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}