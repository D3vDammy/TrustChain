// Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrustChain.Data;
using TrustChain.DTOs.Auth;
using TrustChain.Models;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class AuthService : IAuthService
{
    
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private static readonly Dictionary<string, OtpEntry> _otpStore = new();

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<string> RegisterAsync(RegisterRequestDto dto)
    {
        var exists = await _db.Voters.AnyAsync(v => v.NIN == dto.NIN);
        if (exists)
            throw new Exception("This NIN is already registered.");

        var voter = new Voter
        {
            NIN = dto.NIN,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            StateOfOrigin = dto.StateOfOrigin,
            HasVoted = false,
            IsVerified = false,
            
        };

        _db.Voters.Add(voter);
        await _db.SaveChangesAsync();

        return "Registration successful. Please verify your phone number.";
    }

    public async Task<string> SendOtpAsync(SendOtpDto dto)
    {
        var voter = await _db.Voters
            .FirstOrDefaultAsync(v => v.NIN == dto.NIN
                               && v.PhoneNumber == dto.PhoneNumber);

        if (voter == null)
            throw new Exception("Voter not found. Please register first.");

        var otp = "1234"; // Simulated OTP

        _otpStore[dto.PhoneNumber] = new OtpEntry
        {
            PhoneNumber = dto.PhoneNumber,
            Code = otp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        Console.WriteLine($"[OTP SIMULATION] Code {otp} → {dto.PhoneNumber}");

        return $"OTP sent to {MaskPhone(dto.PhoneNumber)}. Expires in 5 minutes.";
    }

    public async Task<LoginResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
    {
        if (!_otpStore.TryGetValue(dto.PhoneNumber, out var otpEntry))
            throw new Exception("OTP not found or expired. Request a new one.");

        if (otpEntry.IsUsed)
            throw new Exception("OTP already used.");

        if (otpEntry.ExpiresAt < DateTime.UtcNow)
        {
            _otpStore.Remove(dto.PhoneNumber);
            throw new Exception("OTP has expired. Request a new one.");
        }

        if (otpEntry.Code != dto.OTP)
            throw new Exception("Invalid OTP. Please try again.");

        _otpStore.Remove(dto.PhoneNumber);

        var voter = await _db.Voters
            
            .FirstOrDefaultAsync(v => v.NIN == dto.NIN)
            ?? throw new Exception("Voter not found.");

        voter.IsVerified = true;
        await _db.SaveChangesAsync();

        var token = GenerateJwtToken(voter);

        return new LoginResponseDto
        {
            Token = token,
            VoterId = voter.Id,
            FullName = voter.FirstName + " " + voter.LastName,
            StateOfOrigin = voter.StateOfOrigin,
            HasVoted = voter.HasVoted
        };
    }

    private string GenerateJwtToken(Voter voter)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, voter.Id.ToString()),
            new Claim(ClaimTypes.Name, voter.FirstName + " " + voter.LastName),
            new Claim("nin", voter.NIN),
            new Claim("hasVoted", voter.HasVoted.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string MaskPhone(string phone)
    {
        if (phone.Length < 4) return phone;
        return phone[..4] + "****" + phone[^3..];
    }
}