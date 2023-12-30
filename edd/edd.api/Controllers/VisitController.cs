using edd.api.Business.VisitService;
using Microsoft.AspNetCore.Mvc;

namespace edd.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitController : Controller
    {
        private readonly IVisitRegisterBusiness _registerBusiness;

        public VisitController(IVisitRegisterBusiness registerBusiness)
        {
            _registerBusiness = registerBusiness;
        }

        [HttpPost]
        public async Task<IActionResult> Register(VisitRequestModel request)
        {
            await _registerBusiness.CommandHandler(request);

            return Ok();
        }
    }
}
