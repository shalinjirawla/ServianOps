namespace ServianOps_Backend.Core.Interfaces
{
    public interface ICurrentTenant
    {
        long? TenantId { get; }
        bool IsAuthenticated { get; }
    }
}
