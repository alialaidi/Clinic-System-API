using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Appointments;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        /*
            POST /appointments                  [Receptionist]
            GET  /appointments                  [Receptionist]
            GET  /appointments/{id}             [Receptionist, Doctor (موعده)]
            PUT  /appointments/{id}/postpone    [Receptionist]
            PUT  /appointments/{id}/cancel      [Receptionist]
            PUT  /appointments/{id}/complete    [Doctor]
         */

        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }



        [HttpGet]
        [EndpointName("GetAllAppointments")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Retrieves all appointments.")]
        [EndpointDescription("Retrieves all appointments ordered by appointment date and time.")]
        [ProducesResponseType(typeof(List<AppointmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AppointmentDTO>>> GetAllAppointments()
        {
            return Ok(await _appointmentService.GetAllAppointmentsAsync());
        }



        [HttpGet("{id:int}")]
        [EndpointName("GetAppointmentById")]
        [Authorize(Roles = "Receptionist,Doctor")]
        [EndpointSummary("Retrieves an appointment by ID.")]
        [EndpointDescription("Receptionists may retrieve any appointment; doctors may retrieve only their own appointments.")]
        [ProducesResponseType(typeof(AppointmentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentDTO>> GetAppointmentById(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated user's ID is missing or invalid.");
            }

            var result = await _appointmentService.GetAppointmentByIdAsync(
                id,
                userId,
                User.IsInRole("Receptionist"));

            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }



        [HttpPost]
        [EndpointName("CreateAppointment")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Creates a new appointment.")]
        [EndpointDescription("Creates an appointment for a client with a doctor at the provided date and time.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO createAppointmentDTO)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated receptionist's user ID is missing or invalid.");
            }

            var result = await _appointmentService.CreateAppointmentAsync(createAppointmentDTO, userId);

            return result.Status switch
            {
                ResultStatus.Success => Created($"api/appointments/{result.Id}", null),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Related resource not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid appointment data", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Appointment cannot be created", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }




        [HttpPut("{id:int}/postpone")]
        [EndpointName("PostponeAppointment")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Postpones an appointment.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostponeAppointment(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id,
            PostponeAppointmentDTO request)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid authenticated user", detail: "The authenticated user's ID is missing or invalid.");
            }

            var result = await _appointmentService.PostponeAppointmentAsync(id, request, userId);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid appointment data", detail: result.Message),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment or receptionist not found", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Appointment cannot be postponed", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }



        [HttpPut("{id:int}/cancel")]
        [EndpointName("CancelAppointment")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Cancels an appointment.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelAppointment(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid authenticated user", detail: "The authenticated user's ID is missing or invalid.");
            }

            var result = await _appointmentService.CancelAppointmentAsync(id, userId);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment or receptionist not found", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Appointment cannot be cancelled", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }



        [HttpPut("{id:int}/complete")]
        [EndpointName("CompleteAppointment")]
        [Authorize(Roles = "Doctor")]
        [EndpointSummary("Marks an appointment as completed.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompleteAppointment(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid authenticated user", detail: "The authenticated user's ID is missing or invalid.");
            }

            var result = await _appointmentService.CompleteAppointmentAsync(id, userId);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment or doctor not found", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Appointment cannot be completed", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }


    }
}
