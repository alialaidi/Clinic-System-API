using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Payments;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public PaymentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost("appointments/{id:int}/payments")]
        [EndpointName("CreatePayment")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Registers a payment for an appointment.")]
        [EndpointDescription("Creates a payment associated with the specified appointment.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePayment(
            [Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id,
            CreatePaymentDTO createPaymentDTO)
        {
            var result = await _appointmentService.CreatePaymentAsync(id, createPaymentDTO);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(StatusCodes.Status201Created, new { id = result.Id }),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid payment data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpGet("appointments/{id:int}/payments")]
        [EndpointName("GetPaymentsByAppointmentId")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Retrieves payments for an appointment.")]
        [EndpointDescription("Retrieves only the payments associated with the specified appointment.")]
        [ProducesResponseType(typeof(List<PaymentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentDTO>>> GetPaymentsByAppointmentId(
            [Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be greater than zero.")] int id)
        {
            var result = await _appointmentService.GetPaymentsByAppointmentIdAsync(id);
            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Appointment not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpGet("payments")]
        [EndpointName("GetAllPayments")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Retrieves all payments.")]
        [ProducesResponseType(typeof(List<PaymentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentDTO>>> GetAllPayments()
        {
            return Ok(await _appointmentService.GetAllPaymentsAsync());
        }
    }
}
