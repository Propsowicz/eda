using CorrelationId.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eda.api.Services.ExceptionService
{
    [ApiController]
    [Route("[controller]")]
    public class ExceptionController : Controller
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ICorrelationContextAccessor _correlationContext;

        public ExceptionController(ISendEndpointProvider sendEndpointProvider, ICorrelationContextAccessor correlationContext)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _correlationContext = correlationContext;
        }

        [HttpPost]
        [Route("noExceptions")]
        public async Task<IActionResult> NoException()
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:exception_command"));
            await sendEndpoint.Send<ExceptionCommand>(new
            {
                CorrelationId = _correlationContext.CorrelationContext.CorrelationId,
                Text = ":)"
            });

            return Ok();
        }

        [HttpPost]
        [Route("retryException")]
        public async Task<IActionResult> ArgumentException()
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:exception_command"));
            await sendEndpoint.Send<ExceptionCommand>(new
            {
                CorrelationId = _correlationContext.CorrelationContext.CorrelationId,
                Text = "exception"
            });

            return Ok();
        }

        [HttpPost]
        [Route("noRetryException")]
        public async Task<IActionResult> NoRetryException()
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:exception_command"));
            await sendEndpoint.Send<ExceptionCommand>(new
            {
                CorrelationId = _correlationContext.CorrelationContext.CorrelationId,
                Text = "0"
            });

            return Ok();
        }
    }
}
