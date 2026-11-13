using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Departments;
using ClinicApplication.Application.DTOs.Doctors;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [EndpointName("GetAllDepartments")]
        [Authorize]
        [EndpointSummary("Retrieves all departments.")]
        [EndpointDescription("Retrieves a list of all departments in the system.")]
        [ProducesResponseType(typeof(List<DepartmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments()
        {
            return Ok(await _departmentService.GetAllDepartmentsAsync());
        }


        [HttpPut("{id:int}")]
        [EndpointName("UpdateDepartment")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates an existing department.")]
        [EndpointDescription("Updates the details of an existing department in the system.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDepartment(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Department ID must be greater than zero.")] int id,
            UpdateDepartmentDTO updateDepartmentDTO)
        {
            var result = await _departmentService.UpdateDepartmentAsync(id, updateDepartmentDTO);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Department not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid department data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpDelete("{id:int}")]
        [EndpointName("DeleteDepartment")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Deletes a department without doctors.")]
        [EndpointDescription("Deletes a department after its doctors have been transferred to another department.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDepartment(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Department ID must be greater than zero.")] int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Department not found", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Department has doctors", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpPost("{id:int}/transfer-doctors")]
        [EndpointName("TransferDoctors")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Transfers all doctors to another department.")]
        [EndpointDescription("Moves all doctors from the specified source department to the destination department.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TransferDoctors(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Department ID must be greater than zero.")] int id,
            [FromBody] MoveDoctorRequest request)
        {
            var result = await _departmentService.TransferDoctorsAsync(id, request.DepartmentId);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Department not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid department transfer", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }
        [HttpPost]
        [EndpointName("CreateDepartment")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Creates a new department.")]
        [EndpointDescription("Creates a new department in the system with the provided details.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDTO createDepartmentDTO)
        {
            var result = await _departmentService.CreateDepartmentAsync(createDepartmentDTO);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(StatusCodes.Status201Created, new { id = result.Id }),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid department data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }
    }
}



