// DTOs/Auth/SendOtpDto.cs
namespace TrustChain.DTOs.Auth;

public class SendOtpDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string NIN { get; set; } = string.Empty;
}