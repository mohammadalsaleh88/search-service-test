namespace SearchService.Api.Models
{
    public class ResultModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public PositionModel Position { get; set; }

        public double Distance { get; set; }

        public double Score { get; set; }
    }
}