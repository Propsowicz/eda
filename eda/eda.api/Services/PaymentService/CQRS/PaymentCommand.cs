using csStorage.Builder.csContextBuilder;
using eda.api.Services.PaymentService;
using eda.api.Services.PaymentService.Models;
using MassTransit.Mediator;

namespace eda.api.Services.PaymentService.CQRS
{
    public class PaymentCommand : MediatorRequestHandler<PaymentCommandRequestModel, PaymentCommandResponseModel>
    {
        private readonly csContextBuilder<PaymentEntity> _contextPaymentEntityBuilder;

        public PaymentCommand()
        {
            _contextPaymentEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        protected override async Task<PaymentCommandResponseModel> Handle(PaymentCommandRequestModel request, CancellationToken cancellationToken)
        {
            var paymentEntity = _contextPaymentEntityBuilder.Get().First(x => x.Id == request.Id);
            paymentEntity.IsPayed = true;

            await _contextPaymentEntityBuilder.UpdateAsync(paymentEntity);

            return await Task.FromResult(new PaymentCommandResponseModel()
            {
                IsSuccess = true
            });
        }
    }
}
