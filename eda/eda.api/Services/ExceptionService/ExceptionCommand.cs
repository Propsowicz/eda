using MassTransit;

namespace eda.api.Services.ExceptionService
{
    public class ExceptionCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public string Text { get; set; } = default!;
    }
}
