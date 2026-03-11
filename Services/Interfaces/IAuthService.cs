using System;
using Microsoft.AspNetCore.Identity.Data;
using TrustChain.DTOs.Auth;
using TrustChain.DTOs.Candidate;


namespace TrustChain.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequestDto dto);
    Task<string> SendOtpAsync(SendOtpDto dto);
    Task<LoginResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
}
