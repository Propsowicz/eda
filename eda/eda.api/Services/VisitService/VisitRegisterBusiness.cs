﻿using CorrelationId.Abstractions;
using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Services.VisitService.Models;
using MassTransit;
using Serilog;
using NewVisitRegistered = eda.api.Messages.Events.NewVisitRegistered;

namespace eda.api.Services.VisitService
{
    public class VisitRegisterBusiness : IVisitRegisterBusiness
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly csContextBuilder<VisitEntity> _contextBuilder;
        private readonly ICorrelationContextAccessor _correlationContext;

        public VisitRegisterBusiness(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint,
            ICorrelationContextAccessor correlationContext)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
            _contextBuilder = new csContextBuilder<VisitEntity>();
            _correlationContext = correlationContext;
        }

        public async Task<Guid> CommandHandler(VisitRequestModel request)
        {
            var entity = new VisitEntity
            {
                Id = Guid.NewGuid(),
                VisitDate = request.VisitDate,
                PatientName = request.PatientName,
                PatientEmail = request.PatientEmail,
                DoctorName = request.DoctorName,
                HospitalName = request.HospitalName,
                IsCancelled = false
            };

            if (await SaveVisitModel(entity))
            {
                var correlationId = Guid.Parse(_correlationContext.CorrelationContext.CorrelationId);
                await PublishVisitRegistrationEvent(entity, correlationId);
                await SendCalculateVisitBillCommand(entity, correlationId);
            }
            return entity.Id;
        }

        private async Task<bool> SaveVisitModel(VisitEntity entity)
        {
            try
            {
                await _contextBuilder.AddAsync(entity);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        private async Task PublishVisitRegistrationEvent(VisitEntity entity, Guid correlationId)
        {
            await _publishEndpoint.Publish<NewVisitRegistered>(new
            {
                CorrelationId = correlationId,
                entity.VisitDate,
                entity.PatientName,
                entity.DoctorName,
                entity.HospitalName
            });
        }

        private async Task SendCalculateVisitBillCommand(VisitEntity entity, Guid correlationId)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:send_calculate_visit_bill_command"));
            await sendEndpoint.Send<CalculateVisitBill>(new
            {
                CorrelationId = correlationId,
                VisitId = entity.Id
            });
        }
    }
}