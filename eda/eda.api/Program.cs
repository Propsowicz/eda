using CorrelationId;
using CorrelationId.DependencyInjection;
using eda.api.Services.EmailService;
using eda.api.Services.EmailService.EmailToPatient;
using eda.api.Services.EmailService.NotificationEmail;
using eda.api.Services.PaymentService;
using eda.api.Services.PaymentService.CQRS;
using eda.api.Services.VisitService;
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

builder.Services.AddScoped<IVisitRegisterBusiness, VisitRegisterBusiness>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<PaymentQuery>();
    cfg.AddConsumer<PaymentCommand>();

});

builder.Services.AddMassTransit(x =>
{
    x.SetSnakeCaseEndpointNameFormatter();

    // publish
    x.AddConsumer<SendNotificationEmailToDoctorBusiness>();
    x.AddConsumer<SendNotificationEmailToHospitalBusiness>();

    // send
    x.AddConsumer<PaymentCalculateBusiness>();
    x.AddConsumer<PaymentExpirationPaymentCheckBusiness>();
    x.AddConsumer<SendEmailToPatientBusiness>();

    x.AddDelayedMessageScheduler();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.UseDelayedMessageScheduler();

        cfg.ReceiveEndpoint("publish_visit_notification_event", e =>
        {
            e.ConfigureConsumer<SendNotificationEmailToDoctorBusiness>(ctx);
            e.ConfigureConsumer<SendNotificationEmailToHospitalBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("send_calculate_visit_bill_command", e =>
        {
            e.ConfigureConsumer<PaymentCalculateBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("send_send_payment_expiration_check_command", e =>
        {
            e.ConfigureConsumer<PaymentExpirationPaymentCheckBusiness>(ctx);
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
