namespace SearchService.Api.Models
{
    public class SearchModel
    {
        public string ServiceName { get; set; }

        public PositionModel Geolocation { get; set; }
    }
}