using System.Collections.Generic;
using System.Text.Json;
using Allocation.Exceptions;

namespace Allocation.Config
{
    public class SymplifyConfig
    {
        public int Updated { get; }
        public List<ProjectConfig> Projects { get; set; }

        public SymplifyConfig(int updated, List<ProjectConfig> projects)
        {
            Updated = updated;
            Projects = projects;
        }

        public ProjectConfig FindProjectWithName(string projectName)
        {
            foreach (ProjectConfig project in this.Projects)
            {
                if (project.Name == projectName)
                {
                    return project;
                }
            }

            // TODO Add better message
            throw new ProjectException.NotFoundException("Project not found");
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
