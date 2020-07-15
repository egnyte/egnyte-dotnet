using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Text;
using System;
using Egnyte.Api.Tasks;
using System.Linq;

namespace Egnyte.Api.Tests.Tasks
{
    [TestFixture]
    public class UpdateTaskTests
    {
        private const string EditTaskRequestContent = @"
            {
               ""task"":""Archive the file"",
               ""assignees"":[{""id"":4}],
               ""dueDate"":""2022-08-29""
            }";

        private const string ChangeStatusRequestContent = @"
            {
               ""status"":""COMPLETED""
            }";

        [Test]
        public async Task EditTask_ReturnsSuccess()
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
                                GetTaskDetailsTests.CreatedTaskResponse,
                                Encoding.UTF8,
                                "application/json")
                    });

            var createdTask = GetTaskDetailsTests.GetCreatedTask();
            var taskUpdate = GetUpdatedTask();

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var updatedTask = await egnyteClient.Tasks.UpdateTask(
                createdTask.Id,
                taskUpdate.Task,
                taskUpdate.Assignees,
                taskUpdate.DueDate);

            GetTaskDetailsTests.AssertTasksEqual(createdTask, updatedTask);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/tasks/" + createdTask.Id,
                requestMessage.RequestUri.ToString());

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(EditTaskRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task SetTaskStatus_ReturnsSuccess()
        {
            var httpHandlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(httpHandlerMock);

            httpHandlerMock.SendAsyncFunc =
                (request, cancellationToken) =>
                Task.FromResult(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK
                    });

            var createdTask = GetTaskDetailsTests.GetCreatedTask();

            var egnyteClient = new EgnyteClient("token", "acme", httpClient);
            var updatedTask = await egnyteClient.Tasks.SetTaskStatus(createdTask.Id, Api.Tasks.TaskStatus.Completed);

            var requestMessage = httpHandlerMock.GetHttpRequestMessage();
            Assert.AreEqual(
                "https://acme.egnyte.com/pubapi/v1/tasks/" + createdTask.Id,
                requestMessage.RequestUri.ToString());

            var content = httpHandlerMock.GetRequestContentAsString();
            Assert.AreEqual(
                TestsHelper.RemoveWhitespaces(ChangeStatusRequestContent),
                TestsHelper.RemoveWhitespaces(content));
        }

        [Test]
        public async Task UpdateTask_WithoutId_ThrowsException()
        {
            var httpClient = new HttpClient(new HttpMessageHandlerMock());

            var task = GetUpdatedTask();
            var egnyteClient = new EgnyteClient("token", "acme", httpClient);

            var exception = await AssertExtensions.ThrowsAsync<ArgumentNullException>(
                            () => egnyteClient.Tasks.UpdateTask(null, task.Task, task.Assignees, task.DueDate));

            Assert.IsTrue(exception.Message.Contains("taskId"));
            Assert.IsNull(exception.InnerException);
        }

        internal NewTask GetUpdatedTask()
        {
            return new NewTask
            {
                Task = "Archive the file",
                Assignees = new[] { 4L }.ToList(),
                DueDate = new DateTime(2022, 8, 29, 0, 0, 0, DateTimeKind.Utc)
            };
        }
    }
}