
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrustChain.DTOs.Vote;
using TrustChain.Services.Interfaces;

namespace TrustChain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoteController : ControllerBase
{
    
    private readonly IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpPost("cast")]
    [Authorize]
    public async Task<IActionResult> CastVote([FromBody] CastVoteDto dto)
    {
        var voterId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _voteService.CastVoteAsync(voterId, dto));
    }

    [HttpGet("results/{electionId:int}")]
    public async Task<IActionResult> GetResults(int electionId)
    {
        return Ok(await _voteService.GetLiveResultsAsync(electionId));
    }
}