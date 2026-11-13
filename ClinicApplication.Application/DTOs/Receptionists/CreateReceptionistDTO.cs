using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Receptionists;

public class CreateReceptionistDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "PersonId must be greater than zero.")]
    public int PersonId { get; set; }
}
