using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Egnyte.Api.Common;

namespace Egnyte.Api.Tasks
{
    public class TasksClient : BaseClient
    {
        private const string TasksMethod = "/pubapi/v1/tasks";

        internal TasksClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host)
        {
        }

        /// <summary>
        /// This endpoint is used to create a new file-related task for a user.
        /// </summary>
        /// <param name="task">An object representing task to be created</param>
        /// <returns></returns>
        public async Task<TaskDetails> CreateTask(NewTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (string.IsNullOrWhiteSpace(task.Task))
            {
                throw new ArgumentException(nameof(task.Task) + " is required.", nameof(task));
            }

            if (string.IsNullOrWhiteSpace(task.File))
            {
                throw new ArgumentException(nameof(task.File) + " is required.", nameof(task));
            }

            if (task.Assignees == null || task.Assignees.Count == 0)
            {
                throw new ArgumentException("At least one " + nameof(task.Assignees) + " ID is required.", nameof(task));
            }

            var uriBuilder = BuildUri(TasksMethod);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(MapTaskForRequest(task), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<TaskResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return TasksHelper.MapTaskResponseToTaskDetails(response.Data);
        }

        /// <summary>
        /// This endpoint is used to list all tasks for a given file, or all tasks created by (or assigned to) the user
        /// issuing the API request. Currently, even Administrators can not list all of the tasks for the domain or
        /// tasks "created by" or "assigned to" other users.
        /// </summary>
        /// <param name="groupId">Optional. Persistent ID of a file for which you want to see the tasks. One (or more) of the
        /// three parameters has to be provided: groupId, assigneeId or assignorId.</param>
        /// <param name="assigneeId">Optional. ID of a user the tasks are assigned to. Only ID of the user issuing the API
        /// request is currently permitted. One (or more) of the three parameters has to be provided: groupId,
        /// assigneeId or assignorId.</param>
        /// <param name="assignorId">Optional. ID of a user who created the tasks you want to see. Only ID of the user issuing
        /// the API request is currently permitted. One (or more) of the three parameters has to be provided: groupId,
        /// assigneeId or assignorId.</param>
        /// <param name="limit">Optional. Maximum number of results to return. When the limit is set to 0, the request can be
        /// used to receive the total number of available results (based on the totalCount parameter). The maximum
        /// value of the limit is 50. The default value for the limit is 50 as well.</param>
        /// <param name="offset">Optional. A zero-based index which can be used with the limit to paginate the list of tasks.
        /// Offset defaults to 0.</param>
        /// <param name="sortBy">Optional. Specifies how to sort the results.</param>
        /// <param name="sortDirection">Optional. Specifies how the results should be sorted.</param>
        /// <returns></returns>
        public async Task<TasksList> ListTasks(
            string groupId = null,
            long? assigneeId = null,
            long? assignorId = null,
            int? limit = null,
            int? offset = null,
            string sortBy = null,
            SortDirection sortDirection = SortDirection.Ascending)
        {
            if (limit < 0 || limit > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Non-negative integers between 0 and 50.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var sortValues = new[] { "creationDate", "dueDate" };

            if (!string.IsNullOrWhiteSpace(sortBy) && !sortValues.Contains(sortBy))
            {
                throw new ArgumentException(nameof(sortBy), "Possible values: " + string.Join(", ", sortValues));
            }

            if (string.IsNullOrWhiteSpace(groupId) && !assigneeId.HasValue && !assignorId.HasValue)
            {
                throw new ArgumentNullException(
                    nameof(groupId),
                    string.Format(
                        "Supply one (or more) of the three parameters: {0}, {1} or {2}.",
                        nameof(groupId),
                        nameof(assigneeId),
                        nameof(assignorId)));
            }

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                ListTasksRequestUri(
                    TasksMethod,
                    groupId,
                    assigneeId,
                    assignorId,
                    limit,
                    offset,
                    sortBy,
                    sortDirection));

            var serviceHandler = new ServiceHandler<TasksListResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return TasksHelper.MapTasksListResponse(response.Data);
        }

        /// <summary>
        /// This endpoint is used to get the details for a specific task.
        /// </summary>
        /// <param name="taskId">The id of the task.</param>
        /// <returns></returns>
        public async Task<TaskDetails> GetTaskDetails(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                throw new ArgumentNullException(nameof(taskId));
            }

            var uriBuilder = BuildUri(TasksMethod + "/" + taskId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<TaskResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return TasksHelper.MapTaskResponseToTaskDetails(response.Data);
        }

        /// <summary>
        /// This endpoint is used to change status of a specific task.
        /// </summary>
        /// <param name="taskId">Required. The id of the task to be updated.</param>
        /// <param name="status">Required. New status of the task.</param>
        /// <returns></returns>
        public async Task<bool> SetTaskStatus(string taskId, TaskStatus status)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                throw new ArgumentNullException(nameof(taskId));
            }

            var uriBuilder = BuildUri(TasksMethod + "/" + taskId);
            var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), uriBuilder.Uri)
            {
                Content = new StringContent(MapStatusForRequest(status), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<string>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// This endpoint is used to edit a specific task.
        /// </summary>
        /// <param name="task">Required. The text of the task.</param>
        /// <param name="assignees">Required. ID of an assignee of the task. Only one assignee is allowed now, although
        /// technically, it is placed in the array structure. The reason is that in the future, more than one
        /// assignee might be allowed for a task.</param>
        /// <param name="dueDate">Optional. Due date of the task. In case the dueDate is not specified in this PUT call,
        /// it will be set to null.</param>
        /// <returns></returns>
        public async Task<TaskDetails> UpdateTask(string taskId, string task, List<long> assignees, DateTime? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                throw new ArgumentNullException(nameof(taskId));
            }

            var taskUpdates = new NewTask()
            {
                Task = task,
                Assignees = assignees,
                DueDate = dueDate
            };

            var uriBuilder = BuildUri(TasksMethod + "/" + taskId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, uriBuilder.Uri)
            {
                Content = new StringContent(MapTaskForRequest(taskUpdates), Encoding.UTF8, "application/json")
            };

            var serviceHandler = new ServiceHandler<TaskResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return TasksHelper.MapTaskResponseToTaskDetails(response.Data);
        }

        /// <summary>
        /// This endpoint is used to delete a specific task.
        /// </summary>
        /// <param name="taskId">The id of the task to be deleted.</param>
        /// <returns></returns>
        public async Task<bool> DeleteTask(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                throw new ArgumentNullException(nameof(taskId));
            }

            var uriBuilder = BuildUri(TasksMethod + "/" + taskId);
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<string>(httpClient);
            await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return true;
        }

        private string MapTaskForRequest(NewTask task)
        {
            var builder = new StringBuilder();
            builder
                .Append("{")
                .AppendFormat("\"task\" : \"{0}\",", task.Task)
                .AppendFormat("\"assignees\" : [{0}]", string.Join(",", task.Assignees.Select(a => "{\"id\":" + a + "}")));

            if (!string.IsNullOrEmpty(task.File))
                builder.AppendFormat(", \"file\" : {{\"groupId\":\"{0}\"}}", task.File);

            if (task.DueDate.HasValue)
                builder.AppendFormat(", \"dueDate\": \"{0:yyyy-MM-dd}\"", task.DueDate);

            builder.Append("}");

            return builder.ToString();
        }

        private string MapStatusForRequest(TaskStatus status)
        {
            var builder = new StringBuilder();
            builder
                .Append("{")
                .AppendFormat("\"status\" : \"{0}\"", MapTaskStatus(status));

            builder.Append("}");

            return builder.ToString();
        }

        private Uri ListTasksRequestUri(
            string taskMethod,
            string groupId,
            long? assigneeId,
            long? assignorId,
            int? limit,
            int? offset,
            string sortBy,
            SortDirection sortDirection)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                queryParams.Add("groupId=" + groupId);
            }

            if (assigneeId.HasValue)
            {
                queryParams.Add("assigneeId=" + assigneeId.Value);
            }

            if (assignorId.HasValue)
            {
                queryParams.Add("assignorId=" + assignorId.Value);
            }

            if (limit.HasValue)
            {
                queryParams.Add("limit=" + limit.Value);
            }

            if (offset.HasValue)
            {
                queryParams.Add("offset=" + offset.Value);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                queryParams.Add("sortBy=" + sortBy);
                queryParams.Add("sortDirection=" + (sortDirection == SortDirection.Ascending ? "ASC" : "DESC"));
            }

            var query = string.Join("&", queryParams);

            var uriBuilder = BuildUri(taskMethod, query);

            return uriBuilder.Uri;
        }

        private string MapTaskStatus(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Open:
                    return "OPEN";

                case TaskStatus.Completed:
                    return "COMPLETED";

                default:
                    throw new NotImplementedException("Unhandled status " + status.ToString());
            }
        }
    }
}