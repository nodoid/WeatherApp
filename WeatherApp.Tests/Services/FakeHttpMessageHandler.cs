using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherApps.Tests.Services
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        readonly HttpResponseMessage Response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            Response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseTask = new TaskCompletionSource<HttpResponseMessage>();
            responseTask.SetResult(Response);

            return responseTask.Task;
        }
    }
}