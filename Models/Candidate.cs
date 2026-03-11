// Models/Candidate.cs
using TrustChain.Enums;

namespace TrustChain.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public PoliticalParty Party { get; set; }        
    public string PartyLogoUrl { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string StateOfOrigin { get; set; } = string.Empty;
    public string Manifesto { get; set; } = string.Empty;

    // Foreign key to Election
    public int ElectionId { get; set; }
    public Election? Election { get; set; }
    public string? ElectionType { get; internal set; }

}