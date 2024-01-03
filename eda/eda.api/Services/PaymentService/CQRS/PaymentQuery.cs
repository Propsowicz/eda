using csStorage.Builder.csContextBuilder;
using eda.api.Services.PaymentService;
using eda.api.Services.PaymentService.Models;
using MassTransit.Mediator;

namespace eda.api.Services.PaymentService.CQRS
{
    public class PaymentQuery : MediatorRequestHandler<PaymentQueryRequestModel, PaymentQueryResponseModel>
    {
        private readonly csContextBuilder<PaymentEntity> _contextPaymentEntityBuilder;

        public PaymentQuery()
        {
            _contextPaymentEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        protected override async Task<PaymentQueryResponseModel> Handle(PaymentQueryRequestModel queryRequest, CancellationToken cancellationToken)
        {
            var paymentEntity = _contextPaymentEntityBuilder.Get().First(x => x.Id == queryRequest.Id);

            return await Task.FromResult(new PaymentQueryResponseModel
            {
                Id = queryRequest.Id,
                DateOfPayment = paymentEntity.DateOfPayment,
                VisitPrice = paymentEntity.VisitPrice,
                IsPayed = paymentEntity.IsPayed
            });
        }
    }
}
