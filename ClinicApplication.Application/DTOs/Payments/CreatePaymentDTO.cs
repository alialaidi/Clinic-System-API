using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Payments;

public class CreatePaymentDTO
{
    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Notes { get; set; }
}
