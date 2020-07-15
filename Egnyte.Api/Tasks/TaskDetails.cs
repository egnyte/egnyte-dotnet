using System;
using System.Collections.Generic;

namespace Egnyte.Api.Tasks
{
    public class TaskDetails
    {
        public TaskDetails(
            string id,
            string task,
            DateTime creationDate,
            DateTime? completionDate,
            DateTime? dueDate,
            long? dueDateTimestamp,
            TaskUser assignor,
            List<TaskUser> assignees,
            TaskStatus status,
            TaskFile file)
        {
            this.Id = id;
            this.Task = task;
            this.CreationDate = creationDate;
            this.CompletionDate = completionDate;
            this.DueDate = dueDate;
            this.DueDateTimestamp = dueDateTimestamp;
            this.Assignor = assignor;
            this.Assignees = assignees;
            this.Status = status;
            this.File = file;
        }

        public string Id { get; set; }

        public string Task { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public DateTime? DueDate { get; set; }

        public long? DueDateTimestamp { get; set; }

        public TaskUser Assignor { get; set; }

        public List<TaskUser> Assignees { get; set; }

        public TaskStatus Status { get; set; }

        public TaskFile File { get; set; }
    }
}