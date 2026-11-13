using System.Security.Claims;
using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Appointments;
using ClinicApplication.Application.DTOs.Clients;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        /*
            GET /me/profile
            GET /me/appointments
            GET /me/appointments/{id}
            GET /me/medical-records
            GET /me/prescriptions
         */

        private readonly IClientService _clientService;

        public MeController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("profile")]
        [EndpointName("GetMyProfile")]
        [Authorize(Roles = "Client")]
        [EndpointSummary("Retrieves the authenticated client's profile.")]
        [EndpointDescription("Retrieves profile information for the client associated with the authenticated user.")]
        [ProducesResponseType(typeof(ClientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientDTO>> GetMyProfile()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated user's ID is missing or invalid.");
            }

            var result = await _clientService.GetMyProfileAsync(userId);
            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Client profile not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpGet("appointments")]
        [EndpointName("GetMyAppointments")]
        [Authorize(Roles = "Client")]
        [EndpointSummary("Retrieves the authenticated client's appointments.")]
        [EndpointDescription("Retrieves appointments associated with the client linked to the authenticated user.")]
        [ProducesResponseType(typeof(List<AppointmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AppointmentDTO>>> GetMyAppointments()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated user's ID is missing or invalid.");
            }

            return Ok(await _clientService.GetMyAppointmentsAsync(userId));
        }

        [HttpGet("appointments/{id:int}")]
        [EndpointName("GetMyAppointmentById")]
        [Authorize(Roles = "Client")]
        [EndpointSummary("Retrieves one of the authenticated client's appointments.")]
        [EndpointDescription("Retrieves an appointment only if it belongs to the client linked to the authenticated user.")]
        [ProducesResponseType(typeof(AppointmentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentDTO>> GetMyAppointmentById(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated user's ID is missing or invalid.");
            }

            var result = await _clientService.GetMyAppointmentByIdAsync(id, userId);
            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpGet("medical-records")]
        [EndpointName("GetMyMedicalRecords")]
        [Authorize(Roles = "Client")]
        [EndpointSummary("Retrieves the authenticated client's medical records.")]
        [EndpointDescription("Retrieves the medical records associated with the client linked to the authenticated user.")]
        [ProducesResponseType(typeof(List<ClientMedicalRecordDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientMedicalRecordDTO>>> GetMyMedicalRecords()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated user's ID is missing or invalid.");
            }

            return Ok(await _clientService.GetMyMedicalRecordsAsync(userId));
        }

        [HttpGet("prescriptions")]
        [EndpointName("GetMyPrescriptions")]
        [Authorize(Roles = "Client")]
        [EndpointSummary("Retrieves the authenticated client's prescriptions.")]
        [EndpointDescription("Retrieves prescriptions and medication items associated with the client linked to the authenticated user.")]
        [ProducesResponseType(typeof(List<PrescriptionHistoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PrescriptionHistoryDTO>>> GetMyPrescriptions()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated user's ID is missing or invalid.");
            }

            return Ok(await _clientService.GetMyPrescriptionsAsync(userId));
        }
    }
}
