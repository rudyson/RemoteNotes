using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPECS.ISTK.Service.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var isRegistered = await _authService.RegisterAsync(request, cancellationToken);

        if (!isRegistered)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            if (response is null)
            {
                _logger.LogWarning("Password wrong for: @{Username}", request.Username);
                return BadRequest();
            }
            return Ok(response);
        }
        catch (Exception)
        {
            _logger.LogWarning("Login attempt for user not exists: @{Username}", request.Username);
            return BadRequest();
        }
    }

    [Authorize]
    [HttpGet("TestAuth")]
    public async Task<IActionResult> TestAuth(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}
