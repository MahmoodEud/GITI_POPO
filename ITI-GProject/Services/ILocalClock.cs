namespace ITI_GProject.Services
{
    public interface ILocalClock
    {
        DateTime Now();              
        DateTimeOffset NowOffset();
    }
}
