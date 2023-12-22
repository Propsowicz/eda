namespace edd.api.Business.VisitService
{
    public interface IVisitRegisterBusiness
    {
        Task CommandHandler(VisitEntity entity);
    }
}
