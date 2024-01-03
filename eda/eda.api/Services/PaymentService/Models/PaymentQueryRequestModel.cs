using MassTransit.Mediator;

namespace eda.api.Services.PaymentService.Models
{
    public class PaymentQueryRequestModel : Request<PaymentQueryResponseModel>
    {
        public Guid Id { get; set; }
    }
}
