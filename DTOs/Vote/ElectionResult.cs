namespace TrustChain.DTOs.Vote;   // ← check this line

public class ElectionResultsDto
{
    public int ElectionId { get; set; }
    public string ElectionTitle { get; set; } = string.Empty;
    public int TotalVotesCast { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<VoteResultDto> Results { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}