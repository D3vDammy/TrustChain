using System;

namespace TrustChain.DTOs.Candidate;

public class CandidateDto
{
    public int Id { get; set; }
     public string? FirstName { get; set; } 
     public string? LastName { get; set; } 
  public string? FullName => $"{FirstName} {LastName}";
    public string? Party { get; set; } 
    public string? PartyLogoUrl { get; set; }
    public string? Manifesto { get; set; } = string.Empty;
    public string? ElectionType { get; set; }
}
