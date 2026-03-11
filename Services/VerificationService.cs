using System.Net.Http.Json;
using TrustChain.Services.Interfaces;

namespace TrustChain.Services;

public class VerificationService : IVerificationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<VerificationService> _logger;

    public VerificationService(HttpClient httpClient, IConfiguration config, ILogger<VerificationService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<bool> VerifyNinAsync(string nin)
    {
        try
        {
            var apiKey = _config["VerifyMe:ApiKey"];
            var baseUrl = _config["VerifyMe:BaseUrl"];

            var request = new
            {
                nin = nin
            };

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/verify-nin", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<VerifyMeResponse>();
                return result?.IsValid ?? false;
            }

            _logger.LogWarning("NIN verification failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying NIN: {Nin}", nin);
            return false;
        }
    }

    public Task VerifyNinWithPhoneAsync(string nIN, string phoneNumber)
    {
        throw new NotImplementedException();
    }

    private class VerifyMeResponse
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
    }
}