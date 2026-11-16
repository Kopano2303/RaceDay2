
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RaceDayApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string endpoint,
            object? data = null,
            string? token = null)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var request = new HttpRequestMessage(method, endpoint);

            if (data != null)
            {
                request.Content = JsonContent.Create(data);
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await client.SendAsync(request);
        }
    }
}
