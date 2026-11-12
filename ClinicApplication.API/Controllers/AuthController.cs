using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Auth;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointName("Login")]
    [EndpointSummary("Authenticates a user and returns a JWT.")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);

        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Data),
            ResultStatus.Unauthorized => Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid credentials", detail: result.Message),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
        };
    }



    [HttpPost("change-password")]
    [Authorize]
    [EndpointName("ChangePassword")]
    [EndpointSummary("Changes the authenticated user's password.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDTO request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid authenticated user",
                detail: "The authenticated user's ID is missing or invalid.");
        }

        var result = await _authService.ChangePasswordAsync(userId, request);

        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid password change", detail: result.Message),
            ResultStatus.Unauthorized => Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Password verification failed", detail: result.Message),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
        };
    }



    [HttpGet("me")]
    [Authorize]
    [EndpointName("GetCurrentUser")]
    [EndpointSummary("Retrieves the authenticated user's profile.")]
    [ProducesResponseType(typeof(CurrentUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CurrentUserDTO>> GetCurrentUser()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid authenticated user",
                detail: "The authenticated user's ID is missing or invalid.");
        }

        var result = await _authService.GetCurrentUserAsync(userId);

        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Data),
            ResultStatus.Unauthorized => Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid authenticated user", detail: result.Message),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
        };
    }


}
