using eda.api.Services.PaymentService.Models;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace eda.api.Services.PaymentService
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid paymentEntityId)
        {
            return Ok(await _mediator.SendRequest(new PaymentQueryRequestModel
            {
                Id = paymentEntityId,
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Pay(Guid paymentEntityId)
        {
            return Ok(await _mediator.SendRequest(new PaymentCommandRequestModel
            {
                Id = paymentEntityId,
            }));
        }
    }
}
