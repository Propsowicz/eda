using csStorage.Builder.csContextBuilder;
using edd.api.EventModels;
using MassTransit;

namespace edd.api.Business.VisitService
{
    public class VisitRegisterBusiness : IVisitRegisterBusiness
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly csContextBuilder<VisitEntity> _contextBuilder;

        public VisitRegisterBusiness(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _contextBuilder = new csContextBuilder<VisitEntity>();
        }

        public async Task CommandHandler(VisitEntity entity)
        {
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
                entity.Id = Guid.NewGuid();
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
                Message = $"New visit was registered. Patient {entity.PatientName} visit {entity.HospitalName} hospital on {entity.VisitDate} to meet with {entity.DoctorName}."
            });

        }

        private async Task SendBillData(VisitEntity entity)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:send_visit_registered"));
            await sendEndpoint.Send<VisitRegisteredSendModel>(new VisitRegisteredSendModel
            {
                VisitEntity = entity
            });
        }
    }
}
