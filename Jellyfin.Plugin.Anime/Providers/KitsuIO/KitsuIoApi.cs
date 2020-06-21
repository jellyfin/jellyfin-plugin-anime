using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Anime.Providers.KitsuIO
{
    internal class KitsuIoApi
    {
        private static readonly HttpClient _httpClient;
        private const string _apiBaseUrl = "https://kitsu.io/api/edge";
        private static readonly JsonSerializerOptions _serializerOptions;

        static KitsuIoApi()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgent);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));
            
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            _serializerOptions.Converters.Add(new LongToStringConverter());
            _serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public static async Task<ApiSearchResponse> Search_Series(Dictionary<string, string> filters)
        {
            var filterString = string.Join("&",filters.Select(x => $"filter[{x.Key}]={x.Value}"));
            var pageString = "page[limit]=10";
            
            var responseString = await _httpClient.GetStringAsync($"{_apiBaseUrl}/anime?{filterString}&{pageString}");
            var response = JsonSerializer.Deserialize<ApiSearchResponse>(responseString, _serializerOptions);
            return response;
        }
        
        public static async Task<ApiGetResponse> Get_Series(string seriesId)
        {
            var responseString = await _httpClient.GetStringAsync($"{_apiBaseUrl}/anime/{seriesId}?include=genres");
            var response = JsonSerializer.Deserialize<ApiGetResponse>(responseString, _serializerOptions);
            return response;
        }
    }
}