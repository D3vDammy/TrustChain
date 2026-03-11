namespace TrustChain.DTOs.Vote;   // ← check this line

public class VoteResultDto
{
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string PartyLogoUrl { get; set; } = string.Empty;
    public string StateOfOrigin { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
    public double Percentage { get; set; }
    public bool IsLeading { get; set; }
}