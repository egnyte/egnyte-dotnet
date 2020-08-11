using System;
using System.Collections.Generic;

namespace Egnyte.Api.Tasks
{
    public class NewTask
    {
        /// <summary>
        /// Required. The text of the task.
        /// </summary>
        public string Task { get; set; }

        /// <summary>
        /// Required. Persistent ID of a file for which you want to create the task.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Required. ID of an assignee of the task. Only one assignee is allowed now, although technically, it is
        /// placed in the array structure. The reason is that in the future, more than one assignee might
        /// be allowed for a task.
        /// </summary>
        public List<long> Assignees { get; set; } = new List<long>();

        /// <summary>
        ///	Optional. Due date of the task.
        /// </summary>
        public DateTime? DueDate { get; set; }
    }
}