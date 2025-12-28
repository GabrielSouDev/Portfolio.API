using Portfolio.API.Models;

namespace Portfolio.API.Services;

public class JsonBackedStateService : IHostedService
{
    private List<ProjectItem>  _projects = new();

    public async Task ReadProjects()
    {
        //leitura
        //transforma em nó json
        //transforma em classe
        //armazena em jsonbackedstate
    }

    public List<ProjectItem> GetProjects() => _projects;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ReadProjects();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
