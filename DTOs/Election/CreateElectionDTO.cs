
public class CreateElectionDto
{
    public string Title { get; set; }

    // Change THIS from string to the actual enum type
    public string? ElectionType { get; set; }  

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}