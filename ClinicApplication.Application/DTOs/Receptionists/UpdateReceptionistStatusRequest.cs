using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Receptionists;

public class UpdateReceptionistStatusRequest
{
    [Required(ErrorMessage = "IsActive is required.")]
    public bool? IsActive { get; set; }
}
