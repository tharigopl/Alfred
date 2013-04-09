namespace Bot.Infrastructure.AWS
{
    public interface IInstanceAlert
    {
        bool TryUpdateAlert(int instancesOutOfService);
    }
}