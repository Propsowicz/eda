using CorrelationId;
using CorrelationId.DependencyInjection;
using edd.api.Business.BillService;
using edd.api.Business.EmailService;
using edd.api.Business.VisitService;
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
            .ConfigureResource(r => r.AddService("edd.api"))
            .WithTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddSource("MassTransit");
                builder.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri("http://localhost:4317");
                });
            });

builder.Services.AddScoped<IVisitRegisterBusiness, VisitRegisterBusiness>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMassTransit(x =>
{
    x.SetSnakeCaseEndpointNameFormatter();

    // publish
    x.AddConsumer<SendEmailToDoctorBusiness>();
    x.AddConsumer<SendEmailToHospitalBusiness>();

    //send
    x.AddConsumer<BillServiceBusiness>();
    x.AddConsumer<SendReturnEmailBusiness>();


    //docker: https://code-maze.com/masstransit-rabbitmq-aspnetcore/
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ReceiveEndpoint("publish_visit_registered", e =>
        {
            e.ConfigureConsumer<SendEmailToDoctorBusiness>(ctx);
            e.ConfigureConsumer<SendEmailToHospitalBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("send_visit_registered", e =>
        {
            e.ConfigureConsumer<BillServiceBusiness>(ctx);
            e.UseRawJsonSerializer();
        });

        cfg.ReceiveEndpoint("send_visit_registered_return_email", e =>
        {
            e.ConfigureConsumer<SendReturnEmailBusiness>(ctx);
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
