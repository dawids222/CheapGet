using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.DAL.Services;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibLite.CheapGet.DAL.Tests.Services
{
    [TestFixture]
    public class HttpClientTests
    {
        const string URL = "https://localhost/";

        private CancellationToken _token;

        private MockHttpMessageHandler _httpMessageHandlerMock;
        private System.Net.Http.HttpClient _httpClient;
        private Mock<ISerializer> _serializerMock;

        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _token = new();

            _httpMessageHandlerMock = new();
            _httpClient = new System.Net.Http.HttpClient(_httpMessageHandlerMock);
            _serializerMock = new();

            _client = new(_httpClient, _serializerMock.Object);
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
                .Setup(x => x.Deserialize<Response>(response))
                .Returns(expected);
            _httpMessageHandlerMock
                .When(System.Net.Http.HttpMethod.Get, URL)
                .Respond("application/json", response);

            var result = await _client.GetAsync<Response>(URL, _token);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetAsync_HttpClientThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _httpMessageHandlerMock
                .When(System.Net.Http.HttpMethod.Get, URL)
                .Throw(exception);

            Task act() => _client.GetAsync<Response>(URL, _token);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }

        [Test]
        public void GetAsync_SerializerThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _serializerMock
                .Setup(x => x.Deserialize<Response>(It.IsAny<string>()))
                .Throws(exception);
            _httpMessageHandlerMock
                .When(System.Net.Http.HttpMethod.Get, URL)
                .Respond("application/json", "response");

            Task act() => _client.GetAsync<Response>(URL, _token);

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
}
