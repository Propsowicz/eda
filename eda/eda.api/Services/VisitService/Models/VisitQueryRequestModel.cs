using MassTransit.Mediator;

namespace eda.api.Services.VisitService.Models
{
    public class VisitQueryRequestModel : Request<VisitQueryResponseModel>
    {
        public Guid Id { get; set; }

    }
}
