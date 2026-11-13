using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Clients;
using ClinicApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }


        [HttpGet]
        [EndpointName("GetAllClients")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Retrieves all clients.")]
        [EndpointDescription("Retrieves a list of all clients in the system.")]
        [ProducesResponseType(typeof(List<ClientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ClientDTO>>> GetAllClients()
        {
            return Ok(await _clientService.GetAllClientsAsync());
        }

        [HttpGet("{id:int}")]
        [EndpointName("GetClientById")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Retrieves a client by ID.")]
        [EndpointDescription("Retrieves the details of a specific client by their ID.")]
        [ProducesResponseType(typeof(ClientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientDTO>> GetClientById(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Client ID must be greater than zero.")] int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client is null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "Client not found", detail: $"No client exists with ID {id}.");
            }

            return Ok(client);
        }

        [HttpGet("by-code/{clientCode}")]
        [EndpointName("GetClientByCode")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Retrieves a client by client code.")]
        [EndpointDescription("Retrieves the details of a client using their client code.")]
        [ProducesResponseType(typeof(ClientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientDTO>> GetClientByCode(
            [FromRoute, System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(20)] string clientCode)
        {
            var client = await _clientService.GetClientByCodeAsync(clientCode);
            if (client is null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "Client not found", detail: $"No client exists with code {clientCode}.");
            }

            return Ok(client);
        }

        [HttpPut("{id:int}")]
        [EndpointName("UpdateClient")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Updates an existing client.")]
        [EndpointDescription("Updates the personal details of an existing client.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateClient(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Client ID must be greater than zero.")] int id,
            UpdateClientDTO updateClientDTO)
        {
            var result = await _clientService.UpdateClientAsync(id, updateClientDTO);
            return result.Status switch
            {
                ResultStatus.Success => NoContent(),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Client not found", detail: result.Message),
                ResultStatus.BadRequest => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid client data", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpPost("{id:int}/reset-password")]
        [EndpointName("ResetClientPassword")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Resets a client's password.")]
        [EndpointDescription("Generates a one-time temporary password for a client account.")]
        [ProducesResponseType(typeof(ResetClientPasswordResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResetClientPasswordResponseDTO>> ResetPassword(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Client ID must be greater than zero.")] int id)
        {
            var result = await _clientService.ResetPasswordAsync(id);
            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Client account not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }

        [HttpGet("{id:int}/history")]
        [EndpointName("GetClientHistory")]
        [Authorize(Roles = "Doctor")]
        [EndpointSummary("Retrieves a client's medical history.")]
        [EndpointDescription("Retrieves all appointments and related medical records and prescriptions for a client if the doctor has at least one appointment with them.")]
        [ProducesResponseType(typeof(ClientMedicalHistoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClientMedicalHistoryDTO>> GetClientHistory(
            [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Client ID must be greater than zero.")] int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated doctor's user ID is missing or invalid.");
            }

            var result = await _clientService.GetClientHistoryAsync(id, userId);
            return result.Status switch
            {
                ResultStatus.Success => Ok(result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Client history not found", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }
        [HttpPost]
        [EndpointName("CreateClient")]
        [Authorize(Roles = "Receptionist")]
        [EndpointSummary("Registers a new client.")]
        [EndpointDescription("Creates a client record and user account with a one-time temporary password.")]
        [ProducesResponseType(typeof(CreateClientResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateClient(CreateClientDTO createClientDTO)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) || userId <= 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid authenticated user",
                    detail: "The authenticated receptionist's user ID is missing or invalid.");
            }

            var result = await _clientService.CreateClientAsync(createClientDTO, userId);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(StatusCodes.Status201Created, result.Data),
                ResultStatus.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Receptionist not found", detail: result.Message),
                ResultStatus.Conflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Email already registered", detail: result.Message),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Internal server error")
            };
        }
    }
}






