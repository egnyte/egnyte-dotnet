using Egnyte.Api.Groups;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Groups
{
    [TestFixture]
    public class PartialGroupUpdateTests
    {
        const string PartialGroupUpdateResponseContent = @"
            {
                ""schemas"": [""urn:scim:schemas:core:1.0""],
                ""id"":""e3ba9d90-ebc7-483e-abaa-a84e92480c86"",
                ""displayName"":""Finance"",
                ""members"":
                [
                    {
                        ""username"":""test"",
                        ""value"":9967960066,
                        ""display"":""Test User""
                    }
                ]
            }";

        const string PartialGroupUpdateRequestContent = @"
            {
                ""displayName"":""Finance"",
                ""members"":
                [
                    {""value"":9967960066}
                ]
            }";

        const string PartialGroupUpdateWithMembersDeleteResponseContent = @"
            {
                ""schemas"": [""urn:scim:schemas:core:1.0""],
                ""id"":""e3ba9d90-ebc7-483e-abaa-a84e92480c86"",
                ""displayName"":""Finance"",
                ""members"":
                [
                    {
                        ""username"":""test"",
                        ""value"":9967960069,
                        ""display"":""Test User""
                    },
                    {
                        ""username"":""jdoe"",
                        ""value"": 9967960070,
                        ""display"":""John Doe""
                    }
                ]
            }";

        const string PartialGroupUpdateWithMembersDeleteRequestContent = @"
            {
                ""members"":
                [
                    {""operation"":""delete"", ""value"":9967960066 },
                    {""operation"":""delete"", ""value"":9967960068 }
                ]
            }";

        const string PartialGroupUpdateOnlyDisplayNameResponseContent = @"
            {
                ""schemas"": [""urn:scim:schemas:core:1.0""],
                ""id"":""e3ba9d90-ebc7-483e-abaa-a84e92480c86"",
                ""displayName"":""IT"",
                ""members"":
                [
                    {
                        ""username"":""test"",
                        ""value"":9967960066,
                        ""display"":""Test User""
                    },
                    {
                        ""username"":""jdoe"",
                        ""value"": 9967960068,
                        ""display"":""John Doe""
                    }
                ]
            }";

        const string PartialGroupUpdateOnlyDisplayNameRequestContent = @"
            {
                ""displayName"":""IT""
            }";

        [Test]
        public async Task PartialGroupUpdate_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                                PartialGroupUpdateResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var groupDetails = await egnyteClient.Groups.PartialGroupUpdate(
                "e3ba9d90-ebc7-483e-abaa-a84e92480c86",
                "Finance",
                new List<GroupMember> { new GroupMember(9967960066) });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/groups/e3ba9d90-ebc7-483e-abaa-a84e92480c86",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(new HttpMethod("PATCH"), requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(PartialGroupUpdateRequestContent),
                TestsHelper.RemoveWhitespaces(content));

            Assert.AreEqual(1, groupDetails.Schemas.Count);
            Assert.AreEqual("urn:scim:schemas:core:1.0", groupDetails.Schemas[0]);
            Assert.AreEqual("e3ba9d90-ebc7-483e-abaa-a84e92480c86", groupDetails.Id);
            Assert.AreEqual("Finance", groupDetails.DisplayName);
            Assert.AreEqual(1, groupDetails.Members.Count);

            var firstMember = groupDetails.Members.FirstOrDefault(
                u => u.Value == 9967960066);
            Assert.IsNotNull(firstMember);
            Assert.AreEqual("test", firstMember.Username);
            Assert.AreEqual("Test User", firstMember.Display);
        }
        
        [Test]
        public async Task PartialGroupUpdate_WithMembersDelete_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                                PartialGroupUpdateWithMembersDeleteResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var groupDetails = await egnyteClient.Groups.PartialGroupUpdate(
                "e3ba9d90-ebc7-483e-abaa-a84e92480c86",
                members: new List<GroupMember>
                {
                    new GroupMember(9967960066, true),
                    new GroupMember(9967960068, true)
                });

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/groups/e3ba9d90-ebc7-483e-abaa-a84e92480c86",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(new HttpMethod("PATCH"), requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(PartialGroupUpdateWithMembersDeleteRequestContent),
                TestsHelper.RemoveWhitespaces(content));

            Assert.AreEqual(1, groupDetails.Schemas.Count);
            Assert.AreEqual("urn:scim:schemas:core:1.0", groupDetails.Schemas[0]);
            Assert.AreEqual("e3ba9d90-ebc7-483e-abaa-a84e92480c86", groupDetails.Id);
            Assert.AreEqual("Finance", groupDetails.DisplayName);
            Assert.AreEqual(2, groupDetails.Members.Count);

            var firstMember = groupDetails.Members.FirstOrDefault(
                u => u.Value == 9967960069);
            Assert.IsNotNull(firstMember);
            Assert.AreEqual("test", firstMember.Username);
            Assert.AreEqual("Test User", firstMember.Display);

            var secondMember = groupDetails.Members.FirstOrDefault(
                u => u.Value == 9967960070);
            Assert.IsNotNull(secondMember);
            Assert.AreEqual("jdoe", secondMember.Username);
            Assert.AreEqual("John Doe", secondMember.Display);
        }

        [Test]
        public async Task PartialGroupUpdate_OnlyDisplayName_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                                PartialGroupUpdateOnlyDisplayNameResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var groupDetails = await egnyteClient.Groups.PartialGroupUpdate(
                "e3ba9d90-ebc7-483e-abaa-a84e92480c86", "IT");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/groups/e3ba9d90-ebc7-483e-abaa-a84e92480c86",
                requestMessage.RequestUri.ToString());
            Assert.AreEqual(new HttpMethod("PATCH"), requestMessage.Method);

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(PartialGroupUpdateOnlyDisplayNameRequestContent),
                TestsHelper.RemoveWhitespaces(content));

            Assert.AreEqual(1, groupDetails.Schemas.Count);
            Assert.AreEqual("urn:scim:schemas:core:1.0", groupDetails.Schemas[0]);
            Assert.AreEqual("e3ba9d90-ebc7-483e-abaa-a84e92480c86", groupDetails.Id);
            Assert.AreEqual("IT", groupDetails.DisplayName);
            Assert.AreEqual(2, groupDetails.Members.Count);

            var firstMember = groupDetails.Members.FirstOrDefault(
                u => u.Value == 9967960066);
            Assert.IsNotNull(firstMember);
            Assert.AreEqual("test", firstMember.Username);
            Assert.AreEqual("Test User", firstMember.Display);

            var secondMember = groupDetails.Members.FirstOrDefault(
                u => u.Value == 9967960068);
            Assert.IsNotNull(secondMember);
            Assert.AreEqual("jdoe", secondMember.Username);
            Assert.AreEqual("John Doe", secondMember.Display);
        }

        [Test]
        public async Task PartialGroupUpdate_WhenGroupIdIsEmpty_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                () => egnyteClient.Groups.PartialGroupUpdate(string.Empty));

            Assert.IsTrue(exception.Message.Contains("groupId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
