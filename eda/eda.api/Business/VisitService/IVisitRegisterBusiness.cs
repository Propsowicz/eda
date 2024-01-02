namespace eda.api.Business.VisitService
{
    public interface IVisitRegisterBusiness
    {
        Task CommandHandler(VisitRequestModel request);
    }
}
