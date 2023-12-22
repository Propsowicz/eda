using edd.api.Business.BillService;
using edd.api.Business.EmailService;
using edd.api.Business.VisitService;
using MassTransit;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IVisitRegisterBusiness, VisitRegisterBusiness>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
