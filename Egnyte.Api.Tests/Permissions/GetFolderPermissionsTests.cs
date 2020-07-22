using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Permissions
{
    using System.Net;
    using System.Net.Http;

    using NUnit.Framework;
    using Api.Permissions;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    [TestFixture]
    public class GetFolderPermissionsTests
    {
        const string GetFolderPermissionsResponseContent = @"
            {
                  ""users"": [
                    {
                        ""subject"": ""jsmith"",
                        ""permission"": ""Full""
                    },
                    {
                        ""subject"": ""ajones"",
                        ""permission"": ""Viewer""
                    }
                  ],
                  ""groups"": [
                    {
                        ""subject"": ""All Power Users"",
                        ""permission"": ""Editor""
                    }
                  ]
            }";

        [Test]
        public async Task GetFolderPermissions_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(GetFolderPermissionsResponseContent)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var folderPermissions = await egnyteClient.Permissions.GetFolderPermissions("Shared/myFolder");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Get, requestMessage.Method);

            Assert.AreEqual(2, folderPermissions.Users.Count);
            var firstUser = folderPermissions.Users.FirstOrDefault(u => u.Subject == "jsmith");
            Assert.IsNotNull(firstUser);
            Assert.AreEqual(PermissionType.Full, firstUser.Permission);
            var secondUser = folderPermissions.Users.FirstOrDefault(u => u.Subject == "ajones");
            Assert.IsNotNull(secondUser);
            Assert.AreEqual(PermissionType.Viewer, secondUser.Permission);

            Assert.AreEqual(1, folderPermissions.Groups.Count);
            var group = folderPermissions.Groups.FirstOrDefault(g => g.Subject == "All Power Users");
            Assert.IsNotNull(group);
            Assert.AreEqual(PermissionType.Editor, group.Permission);
        }

        [Test]
        public async Task GetFolderPermissions_ForUsersAndGroups_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(GetFolderPermissionsResponseContent)
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var folderPermissions = await egnyteClient.Permissions.GetFolderPermissions(
                "Shared/myFolder",
                new List<string> { "jsmith", "ajones" },
                new List<string> { "All Power Users" });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Get, requestMessage.Method);

            Assert.AreEqual(2, folderPermissions.Users.Count);
            var firstUser = folderPermissions.Users.FirstOrDefault(u => u.Subject == "jsmith");
            Assert.IsNotNull(firstUser);
            Assert.AreEqual(PermissionType.Full, firstUser.Permission);
            var secondUser = folderPermissions.Users.FirstOrDefault(u => u.Subject == "ajones");
            Assert.IsNotNull(secondUser);
            Assert.AreEqual(PermissionType.Viewer, secondUser.Permission);

            Assert.AreEqual(1, folderPermissions.Groups.Count);
            var group = folderPermissions.Groups.FirstOrDefault(g => g.Subject == "All Power Users");
            Assert.IsNotNull(group);
            Assert.AreEqual(PermissionType.Editor, group.Permission);
        }

        [Test]
        public async Task GetFolderPermissions_WhenPathIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Permissions.GetFolderPermissions(string.Empty));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
