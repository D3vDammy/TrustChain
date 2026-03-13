// DTOs/Auth/RegisterRequestDto.cs
namespace TrustChain.DTOs.Auth;

public class RegisterRequestDto
{
    public string NIN { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
   
   
}