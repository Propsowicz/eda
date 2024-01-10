using CorrelationId;
using CorrelationId.DependencyInjection;
using eda.api.Services.EmailService;
using eda.api.Services.EmailService.EmailToPatient;
using eda.api.Services.EmailService.NotificationEmail;
using eda.api.Services.ExceptionService;
using eda.api.Services.PaymentService;
using eda.api.Services.PaymentService.CQRS;
using eda.api.Services.VisitService.CQRS;
using eda.api.Services.VisitService.Models;
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
    cfg.AddConsumer<VisitQueryCqrs>();
    cfg.AddConsumer<RegisterVisitCommand>();
});

builder.Services.AddMassTransit(x =>
{
    x.SetSnakeCaseEndpointNameFormatter();
    
    // events
    x.AddConsumer<SendNotificationEmailToDoctorBusiness>();
    x.AddConsumer<SendNotificationEmailToHospitalBusiness>();
    x.AddConsumer<ExceptionsConsumer>();

    // commands
    x.AddConsumer<PaymentCalculateBusiness>();
    x.AddConsumer<PaymentExecutionCheckBusiness>();
    x.AddConsumer<SendEmailToPatientBusiness>();
    x.AddConsumer<UpdateVisitCommand>();
    x.AddConsumer<ExceptionBusinessSamples>();
    x.AddConsumer<VisitQueryRequestResponse>();

    x.AddDelayedMessageScheduler();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        #region Retry Policy

        cfg.UseDelayedRedelivery(r =>
        {
            r.Interval(1, TimeSpan.FromSeconds(10));
            r.Ignore(typeof(DivideByZeroException));
        });

        cfg.UseMessageRetry(r =>
        {
            r.Interval(3, TimeSpan.FromSeconds(3));
            r.Ignore(typeof(DivideByZeroException));
        });

        cfg.ReceiveEndpoint("exception_command_failed_event", e =>
        {
            e.ConfigureConsumer<ExceptionsConsumer>(ctx);
        });

        #endregion

        cfg.UseRawJsonSerializer();

        cfg.UseDelayedMessageScheduler();

        cfg.ReceiveEndpoint("get_visit_model_command", e =>
        {            
            e.ConfigureConsumer<VisitQueryRequestResponse>(ctx);
        });               

        cfg.ReceiveEndpoint("visit_notification_event", e =>
        {
            e.ConfigureConsumer<SendNotificationEmailToDoctorBusiness>(ctx);
            e.ConfigureConsumer<SendNotificationEmailToHospitalBusiness>(ctx);
        });

        cfg.ReceiveEndpoint("calculate_visit_price_command", e =>
        {
            e.ConfigureConsumer<PaymentCalculateBusiness>(ctx);
        });

        cfg.ReceiveEndpoint("check_if_payment_executed_command", e =>
        {
            e.ConfigureConsumer<PaymentExecutionCheckBusiness>(ctx);
        });
        
        cfg.ReceiveEndpoint("send_visit_registered_return_email_command", e =>
        {
            e.ConfigureConsumer<SendEmailToPatientBusiness>(ctx);
        });        
        
        cfg.ReceiveEndpoint("send_visit_cancelled_email_command", e =>
        {
            e.ConfigureConsumer<SendEmailToPatientBusiness>(ctx);
        });

        cfg.ReceiveEndpoint("cancel_visit_command", e =>
        {
            e.ConfigureConsumer<UpdateVisitCommand>(ctx);
        });
        
        cfg.ReceiveEndpoint("exception_command", e =>
        {
            e.ConfigureConsumer<ExceptionBusinessSamples>(ctx);
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
