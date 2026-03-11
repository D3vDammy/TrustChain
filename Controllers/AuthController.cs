// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using TrustChain.DTOs.Auth;
using TrustChain.Services.Interfaces;

namespace TrustChain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    //  Register 
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        return Ok(await _authService.RegisterAsync(dto));
    }

    // Send OTP 
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
    {
        return Ok(await _authService.SendOtpAsync(dto));
    }

    //  Verify OTP → returns JWT 
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        return Ok(await _authService.VerifyOtpAsync(dto));
    }
}