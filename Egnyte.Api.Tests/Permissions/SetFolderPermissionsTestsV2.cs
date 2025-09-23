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
    using System.Text.RegularExpressions;

    [TestFixture]
    public class SetFolderPermissionsTestsV2
    {
        const string SetFolderPermissionsRequestContent = @"
            {
                ""userPerms"": {
                    ""jsmith"": ""Viewer"",
                    ""ajones"": ""Viewer""
                }
            }";

        const string SetFolderPermissionsWithUsersAndGroupsRequestContent = @"
            {
                ""userPerms"": {
                    ""jsmith"": ""Full"",
                    ""ajones"": ""Full""
                },
                ""groupPerms"": {
                    ""admins"": ""Full"",
                    ""it_support"": ""Full""
                }
            }";

        const string SetFolderPermissionsWithInheritsPermissionsRequestContent = @"
            {
                ""inheritsPermissions"": ""true""
            }";

        const string SetFolderPermissionsWithInheritsPermissionsAndKeepParentPermissionsRequestContent = @"
            {
                ""inheritsPermissions"": ""false"",
                ""keepParentPermissions"": ""true""
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
            var userList = await egnyteClient.Permissions.SetFolderPermissionsV2(
                "/Shared/myFolder/",
                new List<GroupOrUserPermissions> { new GroupOrUserPermissions("jsmith", PermissionType.Viewer), new GroupOrUserPermissions("ajones", PermissionType.Viewer) });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder",
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
            var userList = await egnyteClient.Permissions.SetFolderPermissionsV2(
                "/Shared/myFolder with ##",
                new List<GroupOrUserPermissions> { new GroupOrUserPermissions("jsmith", PermissionType.Viewer), new GroupOrUserPermissions("ajones", PermissionType.Viewer) });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder with %23%23",
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
            var userList = await egnyteClient.Permissions.SetFolderPermissionsV2(
                "/Shared/myFolder/",
                new List<GroupOrUserPermissions> { new GroupOrUserPermissions("jsmith", PermissionType.Full), new GroupOrUserPermissions("ajones", PermissionType.Full) },
                new List<GroupOrUserPermissions> { new GroupOrUserPermissions("admins", PermissionType.Full), new GroupOrUserPermissions("it_support", PermissionType.Full) });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(SetFolderPermissionsWithUsersAndGroupsRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetFolderPermissions_WithInheritsPermissions_ReturnsSuccess()
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
            var userList = await egnyteClient.Permissions.SetFolderPermissionsV2(
                "/Shared/myFolder/",
                inheritsPermissions: true);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(SetFolderPermissionsWithInheritsPermissionsRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetFolderPermissions_WithInheritsPermissionsAndKeepParentPermissions_ReturnsSuccess()
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
            var userList = await egnyteClient.Permissions.SetFolderPermissionsV2(
                "/Shared/myFolder/",
                inheritsPermissions: false,
                keepParentPermissions: true);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/perms/Shared/myFolder",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(HttpMethod.Post, requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(SetFolderPermissionsWithInheritsPermissionsAndKeepParentPermissionsRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetFolderPermissions_WhenPathIsEmpty_ThrowsArgumentNullException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Permissions.SetFolderPermissionsV2(string.Empty));

            Assert.IsTrue(exception.Message.Contains("path"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task SetFolderPermissions_WhenUsersAndGroupsAreNull_ThrowsArgumentException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Permissions.SetFolderPermissionsV2("myPath"));

            Assert.IsTrue(exception.Message.Contains("userPerms"));
            Assert.IsTrue(exception.Message.Contains("groupPerms"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task SetFolderPermissions_WhenUsersAndGroupsAreEmpty_ThrowsArgumentException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Permissions.SetFolderPermissionsV2(
                    "myPath",
                    new List<GroupOrUserPermissions>(),
                    new List<GroupOrUserPermissions>()));

            Assert.IsTrue(exception.Message.Contains("userPerms"));
            Assert.IsTrue(exception.Message.Contains("groupPerms"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task SetFolderPermissions_WithInheritsPermissionsTrueAndKeepParentPermissions_ThrowsArgumentException()
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

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                () => egnyteClient.Permissions.SetFolderPermissionsV2(
                    "myPath",
                    inheritsPermissions: true,
                    keepParentPermissions: true));

            Assert.IsTrue(exception.Message.Contains("inheritsPermissions"));
            Assert.IsTrue(exception.Message.Contains("keepParentPermissions"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
