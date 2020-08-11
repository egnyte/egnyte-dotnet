using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Egnyte.Api.Tasks;
using NUnit.Framework;

namespace Egnyte.Api.Tests.Tasks
{
    [TestFixture]
    public class ListTasksTests
    {
        private const string ListTasksResponseContent = @"
            {
              ""results"" : [ {
                ""id"" : ""e53ea032-f859-4e4b-94e2-43131fcf7243"",
                ""task"" : ""Check if we need to update the website"",
                ""creationDate"" : 1534460249892,
                ""completionDate"" : null,
                ""dueDate"" : ""2018-08-29"",
                ""dueDateTimestamp"" : 1535612399999,
                ""assignor"" : {
                    ""id"" : 1,
                    ""username"" : ""jdoerr"",
                    ""firstName"" : ""John"",
                    ""lastName"" : ""Doerr"",
                    ""email"" : ""jdoerr@acme.com"",
                    ""status"" : ""ACTIVE"",
                    ""typeName"" : ""admin""
                },
                ""assignees"" : [ {
                    ""id"" : 6,
                    ""username"" : ""ssmith"",
                    ""firstName"" : ""Sean"",
                    ""lastName"" : ""Smith"",
                    ""email"" : ""ssmith@acme.com"",
                    ""status"" : ""ACTIVE"",
                    ""typeName"" : ""user""
                } ],
                ""status"" : ""OPEN"",
                ""file"" : {
                    ""path"" : ""/Shared/Projects/Project1/Private/strategy_slides9_05c.pdf"",
                    ""name"" : ""strategy_slides9_05c.pdf"",
                    ""parentPath"" : ""/Shared/Projects/Project1/Private"",
                    ""entryId"" : ""b6cf1f9d-d413-4122-9af3-ba421233cf2b"",
                    ""groupId"" : ""4ffd13e7-bb21-4fb8-845b-4b9f0689882c"",
                    ""folderId"" : ""29a6d842-3485-45a5-99ed-d714f33e752c"",
                    ""size"" : 75130
                }
            }, {
                ""id"" : ""5c48aebc-abce-44a7-afa1-a4b30c28ab4d"",
                ""task"" : ""Confirm with Marketing"",
                ""creationDate"" : 1534454547000,
                ""completionDate"" : null,
                ""dueDate"" : ""2018-08-23"",
                ""dueDateTimestamp"" : 1535093999999,
                ""assignor"" : {
                    ""id"" : 1,
                    ""username"" : ""jdoerr"",
                    ""firstName"" : ""John"",
                    ""lastName"" : ""Doerr"",
                    ""email"" : ""jdoerr@acme.com"",
                    ""status"" : ""ACTIVE"",
                    ""typeName"" : ""admin""
                },
                ""assignees"" : [ {
                    ""id"" : 4,
                    ""username"" : ""abourne"",
                    ""firstName"" : ""Alice"",
                    ""lastName"" : ""Bourne"",
                    ""email"" : ""abourne@acme.com"",
                    ""status"" : ""ACTIVE"",
                    ""typeName"" : ""user""
                } ],
                ""status"" : ""OPEN"",
                ""file"" : {
                    ""path"" : ""/Shared/Projects/Project1/Private/strategy_slides9_05c.pdf"",
                    ""name"" : ""strategy_slides9_05c.pdf"",
                    ""parentPath"" : ""/Shared/Projects/Project1/Private"",
                    ""entryId"" : ""b6cf1f9d-d413-4122-9af3-ba421233cf2b"",
                    ""groupId"" : ""4ffd13e7-bb21-4fb8-845b-4b9f0689882c"",
                    ""folderId"" : ""29a6d842-3485-45a5-99ed-d714f33e752c"",
                    ""size"" : 75130
                }
            } ],
            ""totalCount"" : 2
        }";

        [Test]
        public async Task ListTasks_ReturnsSuccess()
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
                                ListTasksResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var firstExpectedTask = GetTaskDetailsTests.GetCreatedTask();

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var tasksList = await egnyteClient.Tasks.ListTasks(firstExpectedTask.File.GroupId);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/tasks?groupId=" + firstExpectedTask.File.GroupId,
                requestMessage.RequestUri.ToString());

            Assert.AreEqual(2, tasksList.Count);
            Assert.AreEqual(2, tasksList.Tasks.Count);
            GetTaskDetailsTests.AssertTasksEqual(firstExpectedTask, tasksList.Tasks.First());
        }

        [Test]
        public async Task ListTasks_WithAllParameters_ReturnsSuccess()
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
                                ListTasksResponseContent,
                                Encoding.UTF8,
                                "application/json")
                    });

            var taskDetails = GetTaskDetailsTests.GetCreatedTask();

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var TasksList = await egnyteClient.Tasks.ListTasks(
                groupId: taskDetails.File.GroupId,
                assigneeId: taskDetails.Assignees.First().Id,
                assignorId: taskDetails.Assignor.Id,
                limit: 50,
                offset: 0,
                sortBy: "dueDate",
                sortDirection: SortDirection.Ascending);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                string.Format(
                    "https://acme.egnyte.com/pubapi/v1/tasks?groupId={0}&assigneeId={1}&assignorId={2}&limit=50&offset=0&sortBy=dueDate&sortDirection=ASC",
                    taskDetails.File.GroupId,
                    taskDetails.Assignees.First().Id,
                    taskDetails.Assignor.Id),
                requestMessage.RequestUri.ToString());
        }

        public async Task ListTasks_WithNoParams_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Tasks.ListTasks());

            Assert.IsTrue(exception.Message.Contains("groupId") && exception.Message.Contains("assignorId"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ListTasks_WithInvalidRange_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Tasks.ListTasks(limit: -1));

            Assert.IsTrue(exception.Message.Contains("limit"));
            Assert.IsNull(exception.InnerException);

            exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Tasks.ListTasks(limit: 51));

            Assert.IsTrue(exception.Message.Contains("limit"));
            Assert.IsNull(exception.InnerException);

            exception = await AssertExtensions.ThrowsAsync<ArgumentOutOfRangeException>(
                            () => egnyteClient.Tasks.ListTasks(offset: -1));

            Assert.IsTrue(exception.Message.Contains("offset"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task ListTasks_WithInvalidSort_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                            () => egnyteClient.Tasks.ListTasks(sortBy: "invalid"));

            Assert.IsTrue(exception.Message.Contains("sortBy"));
            Assert.IsNull(exception.InnerException);
        }
    }
}