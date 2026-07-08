namespace ServianOps_Backend.Application.Interfaces
{
    public interface ICurrentUser
    {
        long? UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
