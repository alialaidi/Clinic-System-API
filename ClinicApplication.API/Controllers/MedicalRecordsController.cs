using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.MedicalRecords;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApplication.API.Controllers
{
    [Route("api/medical-records")]
    [ApiController]
    public class MedicalRecordsController : ControllerBase
    {
        /*
            POST /appointments/{id}/medical-record    [Doctor]
            PUT  /medical-records/{id}                [Doctor]
            POST /medical-records/{id}/prescriptions  [Doctor]
         */

        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpPut("{id:int}")]
        [EndpointName("UpdateMedicalRecord")]
        [Authorize(Roles = "Doctor")]
        [EndpointSummary("Updates an existing medical record.")]
        [EndpointDescription("Updates the editable details of a medical record belonging to the authenticated doctor.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMedicalRecord(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Medical record ID must be greater than zero.")] int id,
            UpdateMedicalRecordDTO updateMedicalRecordDTO)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated doctor's user ID is missing or invalid.");
            }

            var result = await _medicalRecordService.UpdateMedicalRecordAsync(id, updateMedicalRecordDTO, userId);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Medical record or doctor not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid medical record data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpPost("{id:int}/prescriptions")]
        [EndpointName("CreatePrescription")]
        [Authorize(Roles = "Doctor")]
        [EndpointSummary("Creates a prescription for a medical record.")]
        [EndpointDescription("Creates a prescription with its items for a medical record belonging to the authenticated doctor.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePrescription(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Medical record ID must be greater than zero.")] int id,
            CreatePrescriptionDTO createPrescriptionDTO)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated doctor's user ID is missing or invalid.");
            }

            var result = await _medicalRecordService.CreatePrescriptionAsync(id, createPrescriptionDTO, userId);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(StatusCodes.Status201Created, new { id = result.Id }),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Medical record or doctor not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid prescription data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }
    }
}
