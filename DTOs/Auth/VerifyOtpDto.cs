// DTOs/Auth/VerifyOtpDto.cs
namespace TrustChain.DTOs.Auth;

public class VerifyOtpDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string OTP { get; set; } = string.Empty;
    public string NIN { get; set; } = string.Empty;
}