using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.UrlShortener.Tests
{
    public abstract class TestBase
    {
        [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
        public static HttpClient Create(HttpStatusCode httpStatusCode, string responseMessage)
        {
            return new HttpClient(new MockHttpMessageHandler(statusCode: httpStatusCode, responseMessage: responseMessage)) {BaseAddress = new Uri("https://localhost")};
        }

        public static HttpClient Create<TResponseDto>(HttpStatusCode httpStatusCode, TResponseDto responseObject, JsonSerializerOptions jsonSerializerOptions)
        {
            return Create(httpStatusCode: httpStatusCode, JsonSerializer.Serialize(value: responseObject, options: jsonSerializerOptions));
        }

        private sealed class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseMessage;
            private readonly HttpStatusCode _statusCode;

            public MockHttpMessageHandler(HttpStatusCode statusCode, string responseMessage)
            {
                this._statusCode = statusCode;
                this._responseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(this._statusCode) {Content = new StringContent(this._responseMessage)};

                return Task.FromResult(httpResponseMessage);
            }
        }
    }
}