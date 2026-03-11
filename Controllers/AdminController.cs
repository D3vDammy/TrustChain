// Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using TrustChain.DTOs.Candidate;
using TrustChain.DTOs.Election;

using TrustChain.Services.Interfaces;

namespace TrustChain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("election")]
    public async Task<IActionResult> CreateElection([FromBody] ElectionDto dto)
    {
        var message = await _adminService.CreateElectionAsync(dto);
        return Ok(new { success = true, message });
    }

    [HttpPost("candidate")]
    public async Task<IActionResult> AddCandidate([FromBody] CandidateDto dto)
    {
        var message = await _adminService.AddCandidateAsync(dto);
        return Ok(new { success = true, message });
    }

    [HttpPut("election/{id:int}/activate")]
    public async Task<IActionResult> ActivateElection(int id)
    {
        var message = await _adminService.ActivateElectionAsync(id);
        return Ok(new { success = true, message });
    }

    [HttpPut("election/{id:int}/close")]
    public async Task<IActionResult> CloseElection(int id)
    {
        var message = await _adminService.CloseElectionAsync(id);
        return Ok(new { success = true, message });
    }

    [HttpGet("voters")]
    public async Task<IActionResult> GetAllVoters()
    {
        var voters = await _adminService.GetAllVotersAsync();
        return Ok(new { success = true, data = voters });
    }

    [HttpGet("stats/{electionId:int}")]
    public async Task<IActionResult> GetStats(int electionId)
    {
        var stats = await _adminService.GetStatsAsync(electionId);
        return Ok(new { success = true, data = stats });
    }

}