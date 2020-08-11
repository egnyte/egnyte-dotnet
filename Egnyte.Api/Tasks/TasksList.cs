using System.Collections.Generic;

namespace Egnyte.Api.Tasks
{
    public class TasksList
    {
        public TasksList(List<TaskDetails> tasks, int count)
        {
            Tasks = tasks;
            Count = count;
        }

        /// <summary>
        /// An array containing the full json information for all tasks
        /// </summary>
        public List<TaskDetails> Tasks { get; private set; }

        /// <summary>
        /// The number of tasks visible to the user in the domain that are returned
        /// </summary>
        public int Count { get; private set; }
    }
}