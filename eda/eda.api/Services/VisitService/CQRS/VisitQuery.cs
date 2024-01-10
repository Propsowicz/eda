using csStorage.Builder.csContextBuilder;
using eda.api.Services.VisitService.Models;
using MassTransit;
using MassTransit.Mediator;

namespace eda.api.Services.VisitService.CQRS
{
    public class VisitQueryCqrs : MediatorRequestHandler<VisitQueryRequestModel, VisitQueryResponseModel>
    {
        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;

        public VisitQueryCqrs()
        {
            _contextVisitEntityBuilder = new csContextBuilder<VisitEntity>();
        }
        protected override async Task<VisitQueryResponseModel> Handle(VisitQueryRequestModel request, CancellationToken cancellationToken)
        {
            var visitEntity = _contextVisitEntityBuilder.Get().First(x => x.Id == request.Id);

            return await Task.FromResult(new VisitQueryResponseModel
            {
                Id = visitEntity.Id,
                VisitDate = visitEntity.VisitDate,
                PatientName = visitEntity.PatientName,
                PatientEmail = visitEntity.PatientEmail,
                DoctorName = visitEntity.DoctorName,
                HospitalName = visitEntity.HospitalName,
                IsCancelled = visitEntity.IsCancelled
            });
        }
    }

    public class VisitQueryRequestResponse : IConsumer<Messages.Commands.VisitQueryRequestModel>
    {
        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;

        public VisitQueryRequestResponse()
        {
            _contextVisitEntityBuilder = new csContextBuilder<VisitEntity>();
        }
        public async Task Consume(ConsumeContext<Messages.Commands.VisitQueryRequestModel> context)
        {
            var visitEntity = _contextVisitEntityBuilder.Get().First(x => x.Id == context.Message.Id);

            await context.RespondAsync<VisitQueryResponseModel>(new VisitQueryResponseModel
            {
                Id = visitEntity.Id,
                VisitDate = visitEntity.VisitDate,
                PatientName = visitEntity.PatientName,
                PatientEmail = visitEntity.PatientEmail,
                DoctorName = visitEntity.DoctorName,
                HospitalName = visitEntity.HospitalName,
                IsCancelled = visitEntity.IsCancelled
            });
        }
    }
}
