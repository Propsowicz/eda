using CorrelationId;
using CorrelationId.DependencyInjection;
using eda.api.Services.EmailService;
using eda.api.Services.EmailService.EmailToPatient;
using eda.api.Services.EmailService.NotificationEmail;
using eda.api.Services.PaymentService;
using eda.api.Services.PaymentService.CQRS;
using eda.api.Services.VisitService;
using eda.api.Services.VisitService.CQRS;
using MassTransit;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultCorrelationId(ctx =>
{
    ctx.UpdateTraceIdentifier = true;
});

builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("eda.api"))
            .WithTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddSource("MassTransit");
                builder.AddOtlpExporter(otelOptions =>
                {
                    otelOptions.Endpoint = new Uri("http://localhost:4317");
                });
            });

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<PaymentQuery>();
    cfg.AddConsumer<PaymentCommand>();
    cfg.AddConsumer<VisitQuery>();
    cfg.AddConsumer<RegisterVisitCommand>();
});

builder.Services.AddMassTransit(x =>
{
    x.SetSnakeCaseEndpointNameFormatter();

    // events
    x.AddConsumer<SendNotificationEmailToDoctorBusiness>();
    x.AddConsumer<SendNotificationEmailToHospitalBusiness>();

    // commands
    x.AddConsumer<PaymentCalculateBusiness>();
    x.AddConsumer<PaymentExecutionCheckBusiness>();
    x.AddConsumer<SendEmailToPatientBusiness>();
    x.AddConsumer<UpdateVisitCommand>();

    x.AddDelayedMessageScheduler();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.UseDelayedMessageScheduler();

        cfg.ReceiveEndpoint("visit_notification_event", e =>
        {
            e.ConfigureConsumer<SendNotificationEmailToDoctorBusiness>(ctx);
            e.ConfigureConsumer<SendNotificationEmailToHospitalBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("calculate_visit_price_command", e =>
        {
            e.ConfigureConsumer<PaymentCalculateBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("check_if_payment_executed_command", e =>
        {
            e.ConfigureConsumer<PaymentExecutionCheckBusiness>(ctx);
            e.UseRawJsonSerializer();
        });
        
        cfg.ReceiveEndpoint("send_visit_registered_return_email_command", e =>
        {
            e.ConfigureConsumer<SendEmailToPatientBusiness>(ctx);
            e.UseRawJsonSerializer();
        });        
        
        cfg.ReceiveEndpoint("send_visit_cancelled_email_command", e =>
        {
            e.ConfigureConsumer<SendEmailToPatientBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("cancel_visit_command", e =>
        {
            e.ConfigureConsumer<UpdateVisitCommand>(ctx);
            e.UseRawJsonSerializer();
        });
    });

});

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCorrelationId();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
