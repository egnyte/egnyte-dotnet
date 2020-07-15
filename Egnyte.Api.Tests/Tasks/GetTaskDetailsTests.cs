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
    public class GetTaskDetailsTests
    {
        internal const string CreatedTaskResponse = @"
            {
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
            }";

        [Test]
        public async Task GetTaskDetails_ReturnsSuccess()
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
                                CreatedTaskResponse,
                                Encoding.UTF8,
                                "application/json")
                    });

            var expectedTask = GetCreatedTask();

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var taskDetails = await egnyteClient.Tasks.GetTaskDetails(expectedTask.Id);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/tasks/" + expectedTask.Id,
                requestMessage.RequestUri.ToString());

            AssertTasksEqual(expectedTask, taskDetails);
        }

        [Test]
        public async Task GetTaskDetails_WithWrongId_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Tasks.GetTaskDetails(null));

            Assert.IsTrue(exception.Message.Contains("taskId"));
            Assert.IsNull(exception.InnerException);
        }

        internal static void AssertTasksEqual(TaskDetails expected, TaskDetails actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Task, actual.Task);
            Assert.AreEqual(expected.CreationDate.ToLocalTime(), actual.CreationDate);
            Assert.AreEqual(expected.CompletionDate, actual.CompletionDate);
            Assert.AreEqual(expected.DueDate, actual.DueDate);
            Assert.AreEqual(expected.DueDateTimestamp, actual.DueDateTimestamp);
            Assert.AreEqual(expected.Status, actual.Status);

            AssertUserEqual(expected.Assignor, actual.Assignor);

            Assert.AreEqual(expected.Assignees.Count, actual.Assignees.Count);
            AssertUserEqual(expected.Assignees.First(), actual.Assignees.First());

            AssertFileEqual(expected.File, actual.File);
        }

        internal static void AssertUserEqual(TaskUser expected, TaskUser actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.Email, actual.Email);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Active, actual.Active);
        }

        internal static void AssertFileEqual(TaskFile expected, TaskFile actual)
        {
            Assert.AreEqual(expected.Path, actual.Path);
            Assert.AreEqual(expected.ParentPath, actual.ParentPath);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EntryId, actual.EntryId);
            Assert.AreEqual(expected.GroupId, actual.GroupId);
            Assert.AreEqual(expected.FolderId, actual.FolderId);
            Assert.AreEqual(expected.Size, actual.Size);
        }

        internal static TaskDetails GetCreatedTask()
        {
            return GetCreatedTask(GetNewAdminUser(), GetNewStandardUser(), GetNewFile());
        }

        internal static TaskDetails GetCreatedTask(TaskUser assignor, TaskUser assignee, TaskFile file)
        {
            return new TaskDetails(
                "e53ea032-f859-4e4b-94e2-43131fcf7243",
                "Check if we need to update the website",
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(1534460249892),
                null,
                new DateTime(2018, 8, 29, 0, 0, 0, DateTimeKind.Utc),
                1535612399999,
                assignor,
                new[] { assignee }.ToList(),
                Api.Tasks.TaskStatus.Open,
                file);
        }

        internal static TaskUser GetNewAdminUser()
        {
            return new TaskUser(
                1,
                "jdoerr",
                "John",
                "Doerr",
                "jdoerr@acme.com",
                true,
                Api.Users.UserType.Administrator);
        }

        internal static TaskUser GetNewStandardUser()
        {
            return new TaskUser(
                6,
                "ssmith",
                "Sean",
                "Smith",
                "ssmith@acme.com",
                true,
                Api.Users.UserType.StandardUser);
        }

        internal static TaskFile GetNewFile()
        {
            return new TaskFile()
            {
                Path = "/Shared/Projects/Project1/Private/strategy_slides9_05c.pdf",
                Name = "strategy_slides9_05c.pdf",
                ParentPath = "/Shared/Projects/Project1/Private",
                EntryId = "b6cf1f9d-d413-4122-9af3-ba421233cf2b",
                GroupId = "4ffd13e7-bb21-4fb8-845b-4b9f0689882c",
                FolderId = "29a6d842-3485-45a5-99ed-d714f33e752c",
                Size = 75130
            };
        }
    }
}