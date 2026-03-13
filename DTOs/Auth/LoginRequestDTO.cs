using System;

namespace TrustChain.DTOs.Auth;

public class LoginRequestDTO
{
 public string NIN { get; set; } = string.Empty;

 public string FullName { get; set; } = string.Empty;
 public string PhoneNumber { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;

 public string OTP { get; set; } = string.Empty;
}
