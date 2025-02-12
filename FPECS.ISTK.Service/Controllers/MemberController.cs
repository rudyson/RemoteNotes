using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Shared.Requests.Auth;
using FPECS.ISTK.Shared.Requests.MemberProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPECS.ISTK.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MemberController : ControllerBase
{
    private readonly ILogger<MemberController> _logger;
    private readonly IMemberProfileService _memberProfileService;

    public MemberController(ILogger<MemberController> logger, IMemberProfileService memberProfileService)
    {
        _logger = logger;
        _memberProfileService = memberProfileService;
    }


    [HttpGet("{memberId:long}")]
    public async Task<IActionResult> GetProfile(long memberId, CancellationToken cancellationToken = default)
    {
        var profile = await _memberProfileService.GetMemberProfileAsync(memberId, cancellationToken);
        return Ok(profile);
    }

    [HttpPut("{memberId:long}")]
    public async Task<IActionResult> UpdateProfile([FromRoute] long memberId, [FromBody] UpdateMemberProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (memberId != request.Id)
        {
            return BadRequest();
        }

        var profile = await _memberProfileService.UpdateMemberProfileAsync(request, cancellationToken);
        return Ok(profile);
    }
}
