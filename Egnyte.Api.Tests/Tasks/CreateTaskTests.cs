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
    public class CreateTaskTests
    {
        private const string CreateTaskRequestContent = @"
            {
               ""task"":""Check if we need to update the website"",
               ""assignees"":[{""id"":6}],
               ""file"":{""groupId"":""4ffd13e7-bb21-4fb8-845b-4b9f0689882c""},
               ""dueDate"":""2018-08-29""
            }";

        [Test]
        public async Task CreateTask_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Created,
                        Content = new StringContent(
                                GetTaskDetailsTests.CreatedTaskResponse,
                                Encoding.UTF8,
                                "application/json")
                    });

            var expectedTask = GetTaskDetailsTests.GetCreatedTask();
            var newTask = GetNewTask(expectedTask);

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var createdTask = await egnyteClient.Tasks.CreateTask(newTask);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual("https://acme.egnyte.com/pubapi/v1/tasks", requestMessage.RequestUri.ToString());

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(CreateTaskRequestContent),
                TestsHelper.RemoveWhitespaces(content));

            GetTaskDetailsTests.AssertTasksEqual(expectedTask, createdTask);
        }

        [Test]
        public async Task CreateTask_WithNullParameter_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Tasks.CreateTask(null));

            Assert.IsTrue(exception.Message.Contains("task"));
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public async Task CreateTask_WithoutRequiredFieldsSpecified_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var newTask = GetNewTask();
            newTask.Task = null;
            var exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                            () => egnyteClient.Tasks.CreateTask(newTask));

            Assert.IsTrue(exception.Message.Contains(nameof(newTask.Task)));
            Assert.IsNull(exception.InnerException);

            newTask = GetNewTask();
            newTask.File = null;
            exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                            () => egnyteClient.Tasks.CreateTask(newTask));

            Assert.IsTrue(exception.Message.Contains(nameof(newTask.File)));
            Assert.IsNull(exception.InnerException);

            newTask = GetNewTask();
            newTask.Assignees = null;
            exception = await AssertExtensions.ThrowsAsync<ArgumentException>(
                            () => egnyteClient.Tasks.CreateTask(newTask));

            Assert.IsTrue(exception.Message.Contains(nameof(newTask.Assignees)));
            Assert.IsNull(exception.InnerException);
        }

        internal NewTask GetNewTask()
        {
            return new NewTask
            {
                Task = "Check if we need to update the website",
                Assignees = new[] { 6L }.ToList(),
                DueDate = new DateTime(2018, 8, 29, 0, 0, 0, DateTimeKind.Utc),
                File = "4ffd13e7-bb21-4fb8-845b-4b9f0689882c"
            };
        }

        internal NewTask GetNewTask(TaskDetails expectedTask)
        {
            return new NewTask
            {
                Task = expectedTask.Task,
                Assignees = expectedTask.Assignees.Select(a => a.Id).ToList(),
                DueDate = expectedTask.DueDate,
                File = expectedTask.File.GroupId
            };
        }
    }
}