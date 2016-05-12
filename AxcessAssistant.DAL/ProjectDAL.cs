using System.Collections.Generic;
using System.Linq;
using AxcessAssistant.DAL.Models;

namespace AxcessAssistant.DAL
{
    public class ProjectDAL
    {
        private static List<Project> _projects;

        public ProjectDAL()
        {
            init();
        }

        public Project GetProject(int id)
        {
            return _projects.FirstOrDefault(x => x.ID == id);
        }

        public List<Project> FindProjectsByClientId(int clientId)
        {
            return _projects.FindAll(x => x.ClientID == clientId);
        }

        public void UpdateProject(Project project)
        {
            var p = GetProject(project.ID);
            p.Status = project.Status;
        }

        private void init()
        {
            if (_projects.Count == 0)
            {
                _projects.Add(new Project
                {
                    ID = 1,
                    ClientID = 1,
                    Name = "Tax",
                    Status = "In Preparation"
                });
            }
        }
    }
}

