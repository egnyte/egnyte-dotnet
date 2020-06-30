using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Permissions
{
    using System;
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using System.Text;
    using System.Collections.Generic;
    using Api.Permissions;

    [TestFixture]
    public class SetFolderPermissionsTests
    {
        const string SetFolderPermissionsRequestContent = @"
            {
                ""users"": [
                    ""jsmith"",
                    ""ajones""
                ],
                ""permission"": ""Viewer""
            }";

        const string SetFolderPermissionsWithUsersAndGroupsRequestContent = @"
            {
                ""users"": [
                    ""jsmith"",
                    ""ajones""
                ],
                ""groups"": [
                    ""admins"",
                    ""it_support""
                ],
                ""permission"": ""Full""
            }";

        [Test]
        public async Task SetFolderPermissions_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(string.Empty)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Permissions.SetFolderPermissions(
                "Shared/myFolder/",
                PermissionType.Viewer,
                new List<string> { "jsmith", "ajones" });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/perms/folder/Shared/myFolder/",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(SetFolderPermissionsRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetFolderPermissions_HashCharactersInNameReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(string.Empty)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Permissions.SetFolderPermissions(
                "Shared/myFolder with ##/",
                PermissionType.Viewer,
                new List<string> { "jsmith", "ajones" });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/perms/folder/Shared/myFolder with %23%23/",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(SetFolderPermissionsRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetFolderPermissions_WithUsersAndGroups_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(string.Empty)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Permissions.SetFolderPermissions(
                "Shared/myFolder/",
                PermissionType.Full,
                new List<string> { "jsmith", "ajones" },
                new List<string> { "admins", "it_support" });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/perms/folder/Shared/myFolder/",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(SetFolderPermissionsWithUsersAndGroupsRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetFolderPermissions_WhenPathIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Permissions.SetFolderPermissions(string.Empty, PermissionType.Viewer));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task SetFolderPermissions_WhenUsersAndGroupsAreNull_ThrowsArgumentException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Permissions.SetFolderPermissions("myPath", PermissionType.Viewer));

            Assert.IsTrue(exception.Message.Contains("users"));
            Assert.IsTrue(exception.Message.Contains("groups"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task SetFolderPermissions_WhenUsersAndGroupsAreEmpty_ThrowsArgumentException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Permissions.SetFolderPermissions(
                    "myPath",
                    PermissionType.Viewer,
                    new List<string>(),
                    new List<string>()));

            Assert.IsTrue(exception.Message.Contains("users"));
            Assert.IsTrue(exception.Message.Contains("groups"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
