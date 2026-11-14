using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Doctors;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        /*
            GET   /doctors                    [Admin, Receptionist] *
            GET   /doctors/{id}               [Admin, Receptionist] *
            POST  /doctors                    [Admin]   *
            PUT   /doctors/{id}               [Admin]   *
            PUT   /doctors/{id}/department    [Admin]   *
            PATCH /doctors/{id}/status        [Admin]   *
            GET   /doctors/me/appointments    [Doctor]  *
         */


        private readonly IDoctorService _doctorService;
        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }




        [HttpGet]
        [EndpointName("GetAllDoctors")]
        [Authorize(Roles = "Receptionist,Admin")]
        [EndpointSummary("Retrieves all doctors.")]
        [EndpointDescription("Retrieves a list of all doctors in the system.")]
        [ProducesResponseType(typeof(List<DoctorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DoctorDTO>>> GetAllDoctors()
        {
            return Ok(await _doctorService.GetAllDoctorsAsync());
        }




        [HttpGet("{id:int}")]
        [EndpointName("GetDoctorById")]
        [Authorize(Roles = "Receptionist,Admin")]
        [EndpointSummary("Retrieves a doctor by ID.")]
        [EndpointDescription("Retrieves the details of a specific doctor by their ID.")]
        [ProducesResponseType(typeof(DoctorDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be greater than zero.")] int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "Doctor not found", detail: $"No doctor exists with ID {id}.");
            }
            return Ok(doctor);
        }






        [HttpPost]
        [EndpointName("CreateDoctor")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Creates a new doctor.")]
        [EndpointDescription("Creates a new doctor in the system with the provided details.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDoctor(CreateDoctorDTO createDoctorDTO)
        {
            var result = await _doctorService.CreateDoctorAsync(createDoctorDTO);
            return result.Status switch
            {
                ResultStatus.Success => CreatedAtAction(nameof(GetDoctorById), new { id = result.Id }, null),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Related resource not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid doctor data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }






        [HttpPut("{id:int}")]
        [EndpointName("UpdateDoctor")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates an existing doctor.")]
        [EndpointDescription("Updates the details of an existing doctor in the system.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDoctor(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be greater than zero.")] int id,
            UpdateDoctorDTO updateDoctorDTO)
        {
            var result = await _doctorService.UpdateDoctorAsync(id, updateDoctorDTO);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Doctor or department not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid doctor data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }





        [HttpPut("{id:int}/department")]
        [EndpointName("MoveDoctorToDepartment")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Moves a doctor to a different department.")]
        [EndpointDescription("Moves a doctor to a different department in the system.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MoveDepartment(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be greater than zero.")] int id,
            MoveDoctorRequest request)
        {
            var result = await _doctorService.MoveDoctorAsync(id, request.DepartmentId);

            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Doctor or department not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid department change", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }






        [HttpPatch("{id:int}/status")]
        [EndpointName("UpdateDoctorStatus")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates the status of a doctor.")]
        [EndpointDescription("Updates the active/inactive status of a doctor in the system.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatus(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be greater than zero.")] int id,
            UpdateDoctorStatusRequest request)
        {
            var result = await _doctorService.UpdateStatusAsync(id, request.IsActive.GetValueOrDefault());

            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Doctor not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid status change", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Doctor has upcoming appointments", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }






        [HttpGet("me/appointments")]
        [EndpointName("GetDoctorAppointments")]
        [Authorize(Roles = "Doctor")]
        [EndpointSummary("Retrieves the appointments of the logged-in doctor.")]
        [EndpointDescription("Retrieves a list of appointments for the logged-in doctor, optionally filtered by date and status.")]
        [ProducesResponseType(typeof(List<DoctorAppointmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyAppointments([FromQuery] DateOnly? date,[FromQuery] byte? status)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated doctor's user ID is missing or invalid.");
            }

            var result = await _doctorService.GetMyAppointmentsAsync(userId, date, status);

            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Doctor not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }





    }
}
