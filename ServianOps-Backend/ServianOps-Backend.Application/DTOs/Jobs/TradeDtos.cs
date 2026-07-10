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

    public class TradeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    
    public class TradeListDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
