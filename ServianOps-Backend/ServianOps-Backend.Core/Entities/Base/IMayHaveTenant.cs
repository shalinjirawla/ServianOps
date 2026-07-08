namespace ServianOps_Backend.Core.Entities.Base
{
    public interface IMayHaveTenant
    {
        long? TenantId { get; set; }
    }
}
