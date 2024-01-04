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
        private readonly IMediator _mediator;

        public VisitController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVisitRequestModel request)
        {
            return Ok(await _mediator.SendRequest(request));
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
