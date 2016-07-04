using NUnit.Framework;
using System;
using System.Net.Http;

namespace Egnyte.Api.Tests
{
    [TestFixture]
    public class EgnyteClientTests
    {
        [Test]
        public void CreateClient_WhenSettingTimeout_TimeoutIsSet()
        {
            var timeout = new TimeSpan(0, 15, 0);
            var token = "token";

            var httpClient = new HttpClient();
            var client = new EgnyteClient(token, "domain", httpClient, timeout);
            
            Assert.AreEqual(token, httpClient.DefaultRequestHeaders.Authorization.Parameter);
            Assert.AreEqual("Bearer", httpClient.DefaultRequestHeaders.Authorization.Scheme);

            Assert.AreEqual(httpClient.Timeout, timeout);
            Assert.IsNotNull(client.Files);
            Assert.IsNotNull(client.Groups);
            Assert.IsNotNull(client.Links);
            Assert.IsNotNull(client.Users);
        }

        [Test]
        public void CreateClient_WithEmptyToken_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new EgnyteClient(string.Empty, "domain"));

            Assert.IsTrue(exception.Message.Contains("token"));
        }

        [Test]
        public void CreateClient_WithEmptyDomain_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new EgnyteClient("token", string.Empty));

            Assert.IsTrue(exception.Message.Contains("domain"));
        }
    }
}
