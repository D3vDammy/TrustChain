
namespace TrustChain.DTOs.Auth;

public class VerifyOtpDto
{
    public string Email { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    
}