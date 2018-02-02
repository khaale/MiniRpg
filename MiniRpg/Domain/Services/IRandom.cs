namespace MiniRpg.Domain.Services
{
    public interface IRandom
    {
        double GetNext();
        int GetInRange(int start, int end);
    }
}