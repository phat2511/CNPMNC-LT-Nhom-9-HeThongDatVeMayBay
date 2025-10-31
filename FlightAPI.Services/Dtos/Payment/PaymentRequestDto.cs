using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Payment
{
    public class PaymentRequestDto
    {
        [Required]
        // (Trong DB sếp có: 'QR','Card','BankTransfer','Momo','ZaloPay','Cash')
        public string PaymentMethod { get; set; } = null!;
    }
}