
using TrustChain.Enums;

public class CreateElectionDto
{
    public string? Title { get; set; }
    public ElectionType? ElectionType { get; set; }  

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}