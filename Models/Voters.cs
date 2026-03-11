
using TrustChain.Enums;

namespace TrustChain.Models;

public class Voter                                          // ← Voter not Voters
{
    public int Id { get; set; }                            // ← int not Guid

    // Filled from registration form
    public string NIN { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;  
    public string LastName { get; set; } = string.Empty;   
    public string PhoneNumber { get; set; } = string.Empty; 
    public string StateOfOrigin { get; set; } = string.Empty;
    public Gender Gender { get; set; }                     // ← enum not string

   
    // OTP Verification
    public bool IsVerified { get; set; } = false;

    // When voter casts vote
    public bool HasVoted { get; set; } = false;

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}