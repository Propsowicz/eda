using csStorage.Builder.csContextBuilder;
using eda.api.Services.VisitService.Models;
using MassTransit.Mediator;

namespace eda.api.Services.VisitService.CQRS
{
    public class VisitQuery : MediatorRequestHandler<VisitQueryRequestModel, VisitQueryResponseModel>
    {
        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;

        public VisitQuery()
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
                IsCancelled = visitEntity.IsCancelled
            });
        }
    }
}
