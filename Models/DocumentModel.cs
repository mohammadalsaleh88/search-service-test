namespace SearchService.Api.Models
{
    public class DocumentModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public PositionModel Position { get; set; }
    }
}