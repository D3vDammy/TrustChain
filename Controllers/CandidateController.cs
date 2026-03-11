// Controllers/CandidateController.cs
using Microsoft.AspNetCore.Mvc;
using TrustChain.Services.Interfaces;

namespace TrustChain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CandidateController : ControllerBase
{
   
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet("election/{electionId:int}")]
    public async Task<IActionResult> GetByElection(int electionId)
    {
        return Ok(await _candidateService.GetByElectionAsync(electionId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _candidateService.GetByIdAsync(id));
    }
}