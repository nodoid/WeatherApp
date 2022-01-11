using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApps.Tests.Services
{
    public class FakeHttpContent : HttpContent
    {
        public FakeHttpContent(string content)
        {
            Content = content;
        }

        public string Content { get; set; }

        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(Content);
            await stream.WriteAsync(byteArray, 0, Content.Length);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = Content.Length;
            return true;
        }
    }
}