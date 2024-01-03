using MassTransit.Mediator;

namespace eda.api.Services.PaymentService.Models
{
    public class PaymentCommandRequestModel : Request<PaymentCommandResponseModel>
    {
        public Guid Id { get; set; }
    }
}
