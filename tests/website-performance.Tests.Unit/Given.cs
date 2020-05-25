using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using website_performance.Tests.Unit.Stubs;

namespace website_performance.Tests.Unit
{
    internal static class Given
    {
        internal static class A
        {
            internal static class InvalidRobots
            {

                private static readonly string _invalidRobots = "Garbage all together.";
                
                internal static string Url => "https://www.joaorosa.io/robots.txt";
                internal static HttpMessageHandler HttpMessageHandler => new HttpMessageHandlerStub(HandleWithOkStatus(Url, _invalidRobots));

                private static Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>
                    HandleWithOkStatus(
                        string requestUri,
                        string serializedObject
                    )
                {
                    return (message, token) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(serializedObject),
                        RequestMessage = new HttpRequestMessage
                        {
                            RequestUri = new Uri(requestUri)
                        }
                    });
                }
            }
        }
    }
}