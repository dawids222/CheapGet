using LibLite.CheapGet.Core.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LibLite.CheapGet.DAL.Tests.Services
{
    [TestFixture]
    public class HttpClientTests
    {
        const string URL = "https://localhost/";
        private static readonly Dictionary<string, string> HEADERS = new() { { "key", "value" } };

        private CancellationToken _token;

        private MockHttpMessageHandler _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private ISerializer _serializerMock;

        private DAL.Services.HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _token = new();

            _httpMessageHandlerMock = new();
            _httpClient = new HttpClient(_httpMessageHandlerMock);
            _serializerMock = Substitute.For<ISerializer>();

            _client = new(_httpClient, _serializerMock);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task GetAsync_Success_ReturnsResponse()
        {
            var expected = new Response
            {
                Text = "This is text",
                Number = 1234,
            };
            var response = $"{{\"Text\": \"{expected.Text}\", \"Number\": {expected.Number}}}";
            _serializerMock
                .Deserialize<Response>(response)
                .Returns(expected);
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .Respond("application/json", response);

            var result = await _client.GetAsync<Response>(URL, _token);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task GetAsync_WithHeaders_Success_ReturnsResponse()
        {
            var expected = new Response
            {
                Text = "This is text",
                Number = 1234,
            };
            var response = $"{{\"Text\": \"{expected.Text}\", \"Number\": {expected.Number}}}";
            _serializerMock
                .Deserialize<Response>(response)
                .Returns(expected);
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .With(x => x.ContainsHeaders(HEADERS))
                .Respond("application/json", response);
            var result = await _client.GetAsync<Response>(URL, HEADERS, _token);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetAsync_HttpClientThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .Throw(exception);

            Task act() => _client.GetAsync<Response>(URL, _token);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }

        [Test]
        public void GetAsync_WithHeaders_HttpClientThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .With(x => x.ContainsHeaders(HEADERS))
                .Throw(exception);

            Task act() => _client.GetAsync<Response>(URL, HEADERS, _token);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }

        [Test]
        public void GetAsync_SerializerThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _serializerMock
                .Deserialize<Response>(Arg.Any<string>())
                .Throws(exception);
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .Respond("application/json", "response");

            Task act() => _client.GetAsync<Response>(URL, _token);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }

        [Test]
        public void GetAsync_WithHeaders_SerializerThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _serializerMock
                .Deserialize<Response>(Arg.Any<string>())
                .Throws(exception);
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .With(x => x.ContainsHeaders(HEADERS))
                .Respond("application/json", "response");

            Task act() => _client.GetAsync<Response>(URL, HEADERS, _token);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }

        [Test]
        public async Task GetStringAsync_Success_ReturnContentString()
        {
            var expected = "expected";
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .Respond("text/plain", expected);

            var result = await _client.GetStringAsync(URL, _token);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetStringAsync_HttpClientThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _httpMessageHandlerMock
                .When(HttpMethod.Get, URL)
                .Throw(exception);

            Task act() => _client.GetStringAsync(URL, _token);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }

        internal class Response
        {
            public string Text { get; set; }
            public int Number { get; set; }

            public override bool Equals(object obj)
            {
                return obj is Response response &&
                       Text == response.Text &&
                       Number == response.Number;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Text, Number);
            }
        }
    }

    internal static class HttpRequestMessageExtensions
    {
        public static bool ContainsHeaders(this HttpRequestMessage message, Dictionary<string, string> header)
        {
            return header.All(x => message.Headers.Contains(x.Key) && message.Headers.First(x => x.Key == x.Key).Value.First() == x.Value);
        }
    }
}
