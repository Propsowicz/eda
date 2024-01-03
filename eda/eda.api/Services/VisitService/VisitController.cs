using eda.api.Services.PaymentService.Models;
using eda.api.Services.PaymentService;
using eda.api.Services.VisitService.Models;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace eda.api.Services.VisitService
{
    [ApiController]
    [Route("[controller]")]
    public class VisitController : Controller
    {
        private readonly IVisitRegisterBusiness _registerBusiness;
        private readonly IMediator _mediator;

        public VisitController(IVisitRegisterBusiness registerBusiness, IMediator mediator)
        {
            _registerBusiness = registerBusiness;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Register(VisitRequestModel request)
        {
            return Ok(await _registerBusiness.CommandHandler(request));
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid visitId)
        {
            return Ok(await _mediator.SendRequest(new VisitQueryRequestModel
            {
                Id = visitId,
            }));
        }
    }
}
