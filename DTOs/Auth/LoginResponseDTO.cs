// DTOs/Auth/LoginResponseDto.cs
namespace TrustChain.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int VoterId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string StateOfOrigin { get; set; } = string.Empty;
    public string PollingUnit { get; set; } = string.Empty;
    public string PollingState { get; set; } = string.Empty;
    public bool HasVoted { get; set; }
}