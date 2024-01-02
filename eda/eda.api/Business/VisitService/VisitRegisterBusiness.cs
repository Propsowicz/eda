using CorrelationId.Abstractions;
using csStorage.Builder.csContextBuilder;
using eda.api.EventModels;
using MassTransit;

namespace eda.api.Business.VisitService
{
    public class VisitRegisterBusiness : IVisitRegisterBusiness
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly csContextBuilder<VisitEntity> _contextBuilder;
        private readonly ICorrelationContextAccessor _correlationContext;

        public VisitRegisterBusiness(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint, 
            ICorrelationContextAccessor correlationContext)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _contextBuilder = new csContextBuilder<VisitEntity>();
            _correlationContext = correlationContext;
        }

        public async Task CommandHandler(VisitRequestModel request)
        {
            var entity = VisitEntity.Create(request.VisitDate, request.PatientName, request.PatientEmail, 
                request.DoctorName, request.VisitPrice, request.HospitalName);

            if (await SaveVisitModel(entity))
            {
                await PublishVisitRegistration(entity);
                await SendBillData(entity);
            }
        }

        private async Task<bool> SaveVisitModel(VisitEntity entity)
        {
            try
            {
                await _contextBuilder.AddAsync(entity);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;   
            }
        }

        private async Task PublishVisitRegistration(VisitEntity entity)
        {
            await _publishEndpoint.Publish<VisitRegisteredPublishModel>(new VisitRegisteredPublishModel
            {
                CorrelationId = Guid.Parse(_correlationContext.CorrelationContext.CorrelationId),
                Message = $"New visit was registered. Patient {entity.PatientName} visit {entity.HospitalName} hospital on {entity.VisitDate} to meet with {entity.DoctorName}."
            });;

        }

        private async Task SendBillData(VisitEntity entity)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:send_visit_registered"));
            await sendEndpoint.Send<VisitRegisteredSendModel>(new VisitRegisteredSendModel
            {
                CorrelationId = Guid.Parse(_correlationContext.CorrelationContext.CorrelationId),
                VisitEntity = entity
            });
        }
    }
}
