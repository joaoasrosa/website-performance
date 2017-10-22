using System.Net.Http;

namespace website_performance.Infrastructure
{
    public interface IHttpMessageHandlerFactory
    {
        HttpMessageHandler Get();
    }

    public class HttpMessageHandlerFactory : IHttpMessageHandlerFactory
    {
        public HttpMessageHandler Get()
        {
            return new HttpClientHandler();
        }
    }
}