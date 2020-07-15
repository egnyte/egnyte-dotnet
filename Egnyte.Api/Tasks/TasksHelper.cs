using System;
using System.Linq;
using Egnyte.Api.Users;

namespace Egnyte.Api.Tasks
{
    public static class TasksHelper
    {
        /// <summary>
        ///  For converting unix timestamp to DateTime.
        /// </summary>
        /// <remarks>
        ///  Should use DateTimeOffset.FromUnixTimeMilliseconds under .NET 4.6+
        /// </remarks>
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal static TasksList MapTasksListResponse(TasksListResponse data)
        {
            return new TasksList(
                data.Tasks.Select(t => MapTaskResponseToTaskDetails(t)).ToList(),
                data.Count);
        }

        internal static TaskDetails MapTaskResponseToTaskDetails(TaskResponse data)
        {
            return new TaskDetails(
                data.Id,
                data.Task,
                ConvertFromUnixTimestamp(data.CreationDateTimestamp),
                data.CompletionDateTimestamp.HasValue ? ConvertFromUnixTimestamp(data.CompletionDateTimestamp.Value) : (DateTime?)null,
                data.DueDate = data.DueDate,
                data.DueDateTimestamp,
                MapTaskUserResponseToTaskUser(data.Assignor),
                data.Assignees.Select(u => MapTaskUserResponseToTaskUser(u)).ToList(),
                ParseTaskStatus(data.Status),
                data.File);
        }

        internal static TaskUser MapTaskUserResponseToTaskUser(TaskUserResponse data)
        {
            return new TaskUser(
                data.Id,
                data.Username,
                data.FirstName,
                data.LastName,
                data.Email,
                ParseUserActive(data.Status),
                ParseUserType(data.TypeName));
        }

        private static TaskStatus ParseTaskStatus(string status)
        {
            switch (status.ToLowerInvariant())
            {
                case "open":
                    return TaskStatus.Open;

                case "completed":
                    return TaskStatus.Completed;

                default:
                    throw new NotImplementedException(string.Format("Task status {0} was not expected", status));
            }
        }

        private static UserType ParseUserType(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "admin":
                    return UserType.Administrator;

                case "power":
                    return UserType.PowerUser;

                default:
                    return UserType.StandardUser;
            }
        }

        private static bool ParseUserActive(string status)
        {
            switch (status.ToLower())
            {
                case "active":
                    return true;

                default:
                    return false;
            }
        }

        private static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            return epoch
                .AddMilliseconds(timestamp)
                .ToLocalTime();
        }
    }
}