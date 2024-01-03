using eda.api.Services.VisitService.Models;

namespace eda.api.Services.VisitService
{
    public interface IVisitRegisterBusiness
    {
        Task<Guid> CommandHandler(VisitRequestModel request);
    }
}
