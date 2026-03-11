// Controllers/ElectionController.cs
using Microsoft.AspNetCore.Mvc;
using TrustChain.Services.Interfaces;

namespace TrustChain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElectionController : ControllerBase
{
    private readonly IElectionService _electionService;

    public ElectionController(IElectionService electionService)
    {
        _electionService = electionService;
    }

    // Get active election with all candidates
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        return Ok(await _electionService.GetActiveElectionAsync());
    }

    /// Get all elections
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _electionService.GetAllElectionsAsync());
    }
}