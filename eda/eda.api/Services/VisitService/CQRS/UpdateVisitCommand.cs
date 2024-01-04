using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using MassTransit;

namespace eda.api.Services.VisitService.CQRS
{
    public class UpdateVisitCommand : IConsumer<CancelVisitCommand>
    {
        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;

        public UpdateVisitCommand()
        {
            _contextVisitEntityBuilder = new csContextBuilder<VisitEntity>();
        }

        public async Task Consume(ConsumeContext<CancelVisitCommand> context)
        {
            var visitEntity = _contextVisitEntityBuilder.Get().First(x => x.Id == context.Message.VisitId);
            visitEntity.IsCancelled = true;

            await _contextVisitEntityBuilder.UpdateAsync(visitEntity);
        }
    }
}
