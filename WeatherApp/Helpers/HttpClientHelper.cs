using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WeatherApp.Helpers
{
    public class HttpClientHelper
    {
        public HttpClient SetupClient(int timeOut = -1, bool isMinutes = true, bool acceptMediaType = false, string mediaType = "")
        {
            var client = new HttpClient();
            if (timeOut != -1)
            {
                if (isMinutes)
                    client.Timeout = TimeSpan.FromMinutes(timeOut);
                else
                    client.Timeout = TimeSpan.FromSeconds(timeOut);
            }

            if (acceptMediaType && !string.IsNullOrEmpty(mediaType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            }
               
            return client;
        }
    }
}
