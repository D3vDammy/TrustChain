// DTOs/Election/ElectionDto.cs
using TrustChain.DTOs.Candidate;

namespace TrustChain.DTOs.Election;

public class ElectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;       
    public string Status { get; set; } = string.Empty;     
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<CandidateDto> Candidates { get; set; } = new();
}