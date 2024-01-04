using CorrelationId.Abstractions;
using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Services.VisitService.Models;
using MassTransit;
using MassTransit.Mediator;
using Serilog;
using NewVisitRegistered = eda.api.Messages.Events.NewVisitRegistered;

namespace eda.api.Services.VisitService.CQRS
{
    public class RegisterVisitCommand : MediatorRequestHandler<RegisterVisitRequestModel, RegisterVisitResponseModel>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly csContextBuilder<VisitEntity> _contextBuilder;
        private readonly ICorrelationContextAccessor _correlationContext;

        public RegisterVisitCommand(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint,
            ICorrelationContextAccessor correlationContext)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _contextBuilder = new csContextBuilder<VisitEntity>();
            _correlationContext = correlationContext;
        }
        protected override async Task<RegisterVisitResponseModel> Handle(RegisterVisitRequestModel request, CancellationToken cancellationToken)
        {
            var entity = new VisitEntity
            {
                Id = Guid.NewGuid(),
                VisitDate = request.VisitDate,
                PatientName = request.PatientName,
                PatientEmail = request.PatientEmail,
                DoctorName = request.DoctorName,
                HospitalName = request.HospitalName,
                IsCancelled = false
            };

            if (await SaveVisitModel(entity))
            {
                var correlationId = Guid.Parse(_correlationContext.CorrelationContext.CorrelationId);
                await PublishVisitRegistrationEvent(entity, correlationId);
                await SendCalculateVisitPriceCommand(entity, correlationId);
            }
            return new RegisterVisitResponseModel { Id = entity.Id };
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
                Log.Error(e.Message);
                return false;
            }
        }

        private async Task PublishVisitRegistrationEvent(VisitEntity entity, Guid correlationId)
        {
            await _publishEndpoint.Publish<NewVisitRegistered>(new
            {
                CorrelationId = correlationId,
                entity.VisitDate,
                entity.PatientName,
                entity.DoctorName,
                entity.HospitalName
            });
        }

        private async Task SendCalculateVisitPriceCommand(VisitEntity entity, Guid correlationId)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:calculate_visit_price_command"));
            await sendEndpoint.Send<CalculateVisitPrice>(new
            {
                CorrelationId = correlationId,
                VisitId = entity.Id
            });
        }
    }
}
