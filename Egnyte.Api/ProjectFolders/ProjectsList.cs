using System;
using System.Collections.Generic;
using System.Text;

namespace Egnyte.Api.ProjectFolders
{
    public class ProjectsList
    {
        public ProjectsList(List<ProjectDetails> projects, int count)
        {
            Projects = projects;
            Count = count;
        }

        /// <summary>
        /// An array containing the full json information for projects under criteria
        /// </summary>
        public List<ProjectDetails> Projects { get; private set; }

        /// <summary>
        /// The number of projects returned
        /// </summary>
        public int Count { get; private set; }
    }
}
