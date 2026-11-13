using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Receptionists;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionistsController : ControllerBase
    {
        /*
            POST  /receptionists              [Admin]
            GET   /receptionists              [Admin]
            PUT   /receptionists/{id}         [Admin]
            PATCH /receptionists/{id}/status  [Admin]
         */

        private readonly IReceptionistService _receptionistService;

        public ReceptionistsController(IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        [HttpGet]
        [EndpointName("GetAllReceptionists")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Retrieves all receptionists.")]
        [EndpointDescription("Retrieves a list of all receptionists in the system.")]
        [ProducesResponseType(typeof(List<ReceptionistDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ReceptionistDTO>>> GetAllReceptionists()
        {
            return Ok(await _receptionistService.GetAllReceptionistsAsync());
        }

        [HttpPut("{id:int}")]
        [EndpointName("UpdateReceptionist")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates an existing receptionist.")]
        [EndpointDescription("Updates the personal details of an existing receptionist.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReceptionist(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Receptionist ID must be greater than zero.")] int id,
            UpdateReceptionistDTO updateReceptionistDTO)
        {
            var result = await _receptionistService.UpdateReceptionistAsync(id, updateReceptionistDTO);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Receptionist not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid receptionist data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpPatch("{id:int}/status")]
        [EndpointName("UpdateReceptionistStatus")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates the status of a receptionist.")]
        [EndpointDescription("Updates the active/inactive status of a receptionist account.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatus(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Receptionist ID must be greater than zero.")] int id,
            UpdateReceptionistStatusRequest request)
        {
            var result = await _receptionistService.UpdateStatusAsync(id, request.IsActive.GetValueOrDefault());

            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Receptionist not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid status change", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpPost]
        [EndpointName("CreateReceptionist")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Creates a new receptionist.")]
        [EndpointDescription("Registers an existing person as a receptionist.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateReceptionist(CreateReceptionistDTO createReceptionistDTO)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated admin's user ID is missing or invalid.");
            }

            var result = await _receptionistService.CreateReceptionistAsync(createReceptionistDTO, userId);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(StatusCodes.Status201Created, new { id = result.Id }),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Related resource not found", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Receptionist already exists", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid receptionist data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }
    }
}
