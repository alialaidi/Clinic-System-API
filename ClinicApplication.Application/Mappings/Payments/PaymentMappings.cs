using ClinicApplication.Application.DTOs.Payments;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Payments;

public static class PaymentMappings
{
    public static PaymentDTO ToDTO(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        return new PaymentDTO
        {
            Id = payment.Id,
            AppointmentId = payment.AppointmentId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDate,
            Notes = payment.Notes
        };
    }

    public static Payment ToEntity(this CreatePaymentDTO createPaymentDTO, int appointmentId)
    {
        ArgumentNullException.ThrowIfNull(createPaymentDTO);

        return new Payment
        {
            AppointmentId = appointmentId,
            Amount = createPaymentDTO.Amount,
            PaymentMethod = createPaymentDTO.PaymentMethod,
            Notes = createPaymentDTO.Notes
        };
    }
}
