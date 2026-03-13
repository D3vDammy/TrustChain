
namespace TrustChain.DTOs.Auth;

public class SendOtpDto
{
    public string Email { get; set; } = string.Empty;
    public string NIN { get; set; } = string.Empty;
}