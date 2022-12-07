using System.Collections.Generic;

namespace SearchService.Api.Models
{
    public class SearchResultModel
    {
        public int TotalHits { get; set; }

        public int TotalDocuments { get; set; }

        public List<ResultModel> Results { get; set; }
    }
}