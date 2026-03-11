namespace TrustChain.Services.Interfaces;

public interface IVerificationService
{
    Task<bool> VerifyNinAsync(string nin);
    Task VerifyNinWithPhoneAsync(string nIN, string phoneNumber);
}