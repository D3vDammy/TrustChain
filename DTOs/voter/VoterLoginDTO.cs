// DTOs/Auth/VoterLoginDto.cs
namespace TrustChain.DTOs.Auth;

public class VoterLoginDto
{
    public string NIN { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}