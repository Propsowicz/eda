using eda.api.Services.VisitService.Models;
using Microsoft.AspNetCore.Mvc;

namespace eda.api.Services.VisitService
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
            return Ok(await _registerBusiness.CommandHandler(request));
        }
    }
}
