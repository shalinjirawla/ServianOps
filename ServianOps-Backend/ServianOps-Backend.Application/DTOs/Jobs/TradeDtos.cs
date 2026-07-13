using ServianOps_Backend.Application.DTOs.Base;

namespace ServianOps_Backend.Application.DTOs.Jobs
{
    public class CreateTradeDto
    {
        public string Name { get; set; }
    }

    public class UpdateTradeDto
    {
        public string Name { get; set; }
    }

    public class TradeDto : BaseAuditDto
    {
        public string Name { get; set; }
    }
    
    public class TradeListDto : BaseAuditDto
    {
        public string Name { get; set; }
    }
}
