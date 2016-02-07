using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egnyte.Api.Tests.Groups
{
    [TestFixture]
    public class ShowSingleGroupTests
    {
        const string ListGroupsResponseContent = @"
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
                    },
                    {
                        ""username"":""jdoe"",
                        ""value"": 9967960068,
                        ""display"":""John Doe""
                    }
                ]
            }";

        [Test]
        public async Task ShowSingleGroupTests_ReturnsSuccess()
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
                                ListGroupsResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var userList = await egnyteClient.Groups
                .ShowSingleGruop("e3ba9d90-ebc7-483e-abaa-a84e92480c86");

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v2/groups/e3ba9d90-ebc7-483e-abaa-a84e92480c86",
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(1, userList.Schemas.Count);
            Assert.AreEqual("urn:scim:schemas:core:1.0", userList.Schemas[0]);
            Assert.AreEqual("e3ba9d90-ebc7-483e-abaa-a84e92480c86", userList.Id);
            Assert.AreEqual("Finance", userList.DisplayName);
            Assert.AreEqual(2, userList.Members.Count);

            var firstMember = userList.Members.FirstOrDefault(
                u => u.Value == 9967960066);
            Assert.IsNotNull(firstMember);
            Assert.AreEqual("test", firstMember.Username);
            Assert.AreEqual("Test User", firstMember.Display);

            var secondMember = userList.Members.FirstOrDefault(
                u => u.Value == 9967960068);
            Assert.IsNotNull(secondMember);
            Assert.AreEqual("jdoe", secondMember.Username);
            Assert.AreEqual("John Doe", secondMember.Display);
        }

        [Test]
        public async Task ShowSingleGroupTests_WhenGroupIdIsEmpty_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Groups.ShowSingleGruop(string.Empty));

            Assert.IsTrue(exception.Message.Contains("groupId"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
