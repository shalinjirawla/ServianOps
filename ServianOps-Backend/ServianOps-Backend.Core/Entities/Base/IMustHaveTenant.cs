namespace ServianOps_Backend.Core.Entities.Base
{
    public interface IMustHaveTenant
    {
        long TenantId { get; set; }
    }
}
