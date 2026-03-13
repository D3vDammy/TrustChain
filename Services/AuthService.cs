
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using TrustChain.Data;
using TrustChain.DTOs.Auth;
using TrustChain.Models;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly IResend _resend;                        

    // OTP store keyed by Email now
    private static readonly Dictionary<string, OtpEntry> _otpStore = new();

    public AuthService(AppDbContext db, IConfiguration config, IResend resend)
    {
        _db = db;
        _config = config;
        _resend = resend;
    }

    // PAGE 1: Register 
    public async Task<string> RegisterAsync(RegisterRequestDto dto)
    {
        var exists = await _db.Voters.AnyAsync(v => v.NIN == dto.NIN);
        if (exists)
            throw new Exception("This NIN is already registered.");

        var emailExists = await _db.Voters.AnyAsync(v => v.Email == dto.Email);
        if (emailExists)
            throw new Exception("This email is already registered.");

        var voter = new Voter
        {
            NIN = dto.NIN,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            HasVoted = false,
            IsVerified = false,
           
        };

        _db.Voters.Add(voter);
        await _db.SaveChangesAsync();

        return "Registration successful. Please verify your email.";
    }

    // Send OTP via Resend 
    public async Task<string> SendOtpAsync(SendOtpDto dto)
    {
        var voter = await _db.Voters
            .FirstOrDefaultAsync(v => v.NIN == dto.NIN
                               && v.Email == dto.Email);

        if (voter == null)
            throw new Exception("Voter not found. Please register first.");

        // Generate random 6 digit OTP
        var otp = new Random().Next(100000, 999999).ToString();

        // Store OTP keyed by email
        _otpStore[dto.Email] = new OtpEntry
        {
            Email = dto.Email,   
            Code = otp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        // Send OTP email via Resend
        var message = new EmailMessage();
        message.From = "TrustChain <onboarding@resend.dev>";
        message.To.Add(dto.Email);
        message.Subject = "Your TrustChain Voting OTP";
        message.HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #1a1a2e;'>🗳️ TrustChain Voting</h2>
                <p>Hello <strong>{voter.FirstName} {voter.LastName}</strong>,</p>
                <p>Your One-Time Password (OTP) to verify your identity is:</p>
                <div style='background: #f4f4f4; padding: 20px; text-align: center; 
                            border-radius: 8px; margin: 20px 0;'>
                    <h1 style='color: #008751; font-size: 48px; letter-spacing: 8px;'>
                        {otp}
                    </h1>
                </div>
                <p>This OTP expires in <strong>5 minutes</strong>.</p>
                <p style='color: #999; font-size: 12px;'>
                    If you did not request this, please ignore this email.
                </p>
                <hr/>
                <p style='color: #008751;'><strong>TrustChain</strong> — 
                Securing Nigeria's Democracy 🇳🇬</p>
            </div>";

        await _resend.EmailSendAsync(message);

        Console.WriteLine($"[RESEND] OTP {otp} sent to {dto.Email}");

        return $"OTP sent to {MaskEmail(dto.Email)}. Expires in 5 minutes.";
    }

    //  Verify OTP → JWT 
    public async Task<LoginResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
    {
        if (!_otpStore.TryGetValue(dto.Email, out var otpEntry))
            throw new Exception("OTP not found or expired. Request a new one.");

        if (otpEntry.IsUsed)
            throw new Exception("OTP already used.");

        if (otpEntry.ExpiresAt < DateTime.UtcNow)
        {
            _otpStore.Remove(dto.Email);
            throw new Exception("OTP has expired. Request a new one.");
        }

        if (otpEntry.Code != dto.OTP)
            throw new Exception("Invalid OTP. Please try again.");

        _otpStore.Remove(dto.Email);
        var voter = await _db.Voters
            .FirstOrDefaultAsync(v => v.Email == dto.Email)
            ?? throw new Exception("Voter not found.");  

        var token = GenerateJwtToken(voter);

        return new LoginResponseDto
        {
            Token = token,
            VoterId = voter.Id,
            FirstName = voter.FirstName,
            LastName = voter.LastName,
            HasVoted = voter.HasVoted
        };
    }

    // Generate JWT
    private string GenerateJwtToken(Voter voter)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, voter.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{voter.FirstName} {voter.LastName}"),
            new Claim(ClaimTypes.Email, voter.Email),
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

    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return email;
        var name = parts[0];
        var domain = parts[1];
        var masked = name.Length <= 2 ? name : name[..2] + "****";
        return $"{masked}@{domain}";
    }
}