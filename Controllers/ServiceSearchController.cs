using Newtonsoft.Json;
using SearchService.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SearchService.Api.Controllers
{
    public class ServiceSearchController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var values = await LoadJson();

                return Ok(values);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpGet]
        [Route("Search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ServiceName))
                    throw new ArgumentException($"Service Name must be provided!");

                if (model.Geolocation == null || model.Geolocation.Lat <= 0 || model.Geolocation.Lng <= 0)
                    throw new ArgumentException("Geolocation must be provided!");

                var values = await LoadJson();

                var filteredValues = values.Where(v => v.Name.ToLower().Contains(model.ServiceName.ToLower()));

                var modeltoReturn = new SearchResultModel
                {
                    TotalDocuments = values.Count,
                    TotalHits = filteredValues.Count(),
                    Results = filteredValues.Select(d =>
                    {
                        return new ResultModel
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Position = d.Position,
                            Distance = DistanceTo(model.Geolocation.Lat, model.Geolocation.Lng, d.Position.Lat,
                                d.Position.Lng),
                            Score = CalculateSimilarity(d.Name, model.ServiceName)
                        };
                    }).ToList()
                };

                return Ok(modeltoReturn);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public async Task<List<DocumentModel>> LoadJson()
        {
            using (var httpClient = new HttpClient())
            {
                var response =
                    await httpClient.GetAsync(
                        "https://raw.githubusercontent.com/bokadirekt/search-service-test/master/data.json");

                if (!response.IsSuccessStatusCode)
                    return new List<DocumentModel>();

                var result = await response.Content.ReadAsStringAsync();

                var data = result.Replace(Environment.NewLine, "");

                return JsonConvert.DeserializeObject<List<DocumentModel>>(data);
            }
        }

        private double DistanceTo(double fromLatitude, double fromLong, double toLatitude, double toLongitude)
        {
            double rlat1 = Math.PI * fromLatitude / 180;
            double rlat2 = Math.PI * toLatitude / 180;
            double theta = fromLong - toLongitude;
            double rtheta = Math.PI * theta / 180;

            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);

            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1.609344;
        }

        private double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

        private int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        private IHttpActionResult Error(Exception exception)
        {
            if (exception is ArgumentException)
                return Content(HttpStatusCode.BadRequest, exception.Message);

            return Content(HttpStatusCode.InternalServerError, exception.Message);
        }
    }
}