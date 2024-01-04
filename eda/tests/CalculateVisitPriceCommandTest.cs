using eda.api.Messages.Commands;
using eda.api.Services.PaymentService;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace tests
{
    public class CalculateVisitPriceCommandTest
    {
        [Fact]
        public async Task CommandConsumed()
        {
            EndpointConvention.Map<CalculateVisitPrice>(new Uri("queue:calculate_visit_price_command"));
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<PaymentCalculateBusiness>()
                        .Endpoint(e => e.Name = "calculate_visit_price_command");
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            await harness.Bus.Send<CalculateVisitPrice>(new
            {
                CorrelationId = Guid.NewGuid(),
                VisitId = Guid.NewGuid(),
            });

            Assert.True(await harness.Consumed.Any<CalculateVisitPrice>());
        }

        [Fact]
        public async Task CommandNotConsumed()
        {
            EndpointConvention.Map<FakeCalculateVisitPrice>(new Uri("queue:calculate_visit_price_command"));
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<PaymentCalculateBusiness>()
                        .Endpoint(e => e.Name = "calculate_visit_price_command");
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            await harness.Bus.Send<FakeCalculateVisitPrice>(new
            {
                CorrelationId = Guid.NewGuid(),
                VisitId = Guid.NewGuid(),
            });

            Assert.False(await harness.Consumed.Any<FakeCalculateVisitPrice>());
        }
    }

    public class FakeCalculateVisitPrice
    {
        public Guid CorrelationId { get; set; }

        public Guid VisitId { get; set; }
    }
}
