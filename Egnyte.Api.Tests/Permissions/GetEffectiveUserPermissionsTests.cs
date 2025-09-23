using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Permissions
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using Api.Permissions;
    using System;

    [TestFixture]
    public class GetEffectiveUserPermissionsTests
    {
        const string GetUserPermissionsResponseContent = @"
            { ""permission"": ""Owner"" }";

        [Test]
        public async Task GetEffectiveUserPermissions_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{ \"permission\": \"Owner\" }")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var permissions = await egnyteClient.Permissions.GetEffectivePermissionsForUser(
                "mjones", "/Shared/myFolder");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/perms/user/mjones?folder=/Shared/myFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Get, requestMessage.Method);

            Assert.AreEqual(PermissionType.Owner, permissions);
        }
        [Test]
        public async Task GetEffectiveUserPermissions_WithSpecialChars()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{ \"permission\": \"Owner\" }")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var permissions = await egnyteClient.Permissions.GetEffectivePermissionsForUser(
                "mjones", "/Shared/myFolder + anotherFolder");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/perms/user/mjones?folder=/Shared/myFolder %2B anotherFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Get, requestMessage.Method);

            Assert.AreEqual(PermissionType.Owner, permissions);
        }

        [Test]
        public async Task GetEffectiveUserPermissions_WhenUsernameIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Permissions.GetEffectivePermissionsForUser(
                    string.Empty,
                    "path"));

            Assert.IsTrue(exception.Message.Contains("username"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task GetEffectiveUserPermissions_WhenPathIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Permissions.GetEffectivePermissionsForUser(
                    "user",
                    string.Empty));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
