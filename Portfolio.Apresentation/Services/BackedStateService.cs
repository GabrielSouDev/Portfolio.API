using System.Text.Json;
using System.Text.Json.Nodes;
using Portfolio.API.Models;

namespace Portfolio.API.Services;

public class BackedStateService
{
    private IEnumerable<ProjectItem> _projects = new List<ProjectItem>();

    public event Action<IReadOnlyList<ProjectItem>>? OnChange;

    public void SaveProjects(IEnumerable<ProjectItem> projects)
    {
        var snapShot = projects.ToList().AsReadOnly();

        SetProjects(snapShot);
        OnChange?.Invoke(snapShot);
    }

    public IEnumerable<ProjectItem> SetProjects(IReadOnlyList<ProjectItem>  projects) => _projects = projects;

    public IEnumerable<ProjectItem> GetProjects() => _projects;
}