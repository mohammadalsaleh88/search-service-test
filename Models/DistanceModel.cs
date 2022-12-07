using Newtonsoft.Json;

namespace SearchService.Api.Models
{
    class DistanceModel
    {
        public double Distance { get; set; }

        public int Score { get; set; }
    }
}