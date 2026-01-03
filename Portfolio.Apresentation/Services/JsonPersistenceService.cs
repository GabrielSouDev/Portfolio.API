using System.Text.Json;
using System.Text.Json.Nodes;
using Portfolio.API.Models;

namespace Portfolio.API.Services;

public class JsonPersistenceService : IHostedService
{
    private readonly string _path = Path.Combine(Environment.CurrentDirectory, "Storage", "projects.json");
    private readonly BackedStateService _backedState;
    private Action<IReadOnlyList<ProjectItem>>? _handler;

    public JsonPersistenceService(BackedStateService backedState)
    {
        _backedState = backedState;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _handler = OverrideProjects;
        _backedState.OnChange += _handler;

        ReadProjects();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _backedState.OnChange -= _handler;
        return Task.CompletedTask;
    }

    private void ReadProjects()
    {
        var jsonString = File.ReadAllText(_path);

        if (string.IsNullOrEmpty(jsonString)) Console.WriteLine("Arquivo sem conteudo!");

        var projects = JsonSerializer.Deserialize<IEnumerable<ProjectItem>>(jsonString);

        if (projects == null) Console.WriteLine("Erro em Desserialização!");

        var snapshot = projects?.ToList().AsReadOnly();
        _backedState.SetProjects(snapshot);
    }

    public void OverrideProjects(IReadOnlyList<ProjectItem> projects)
    {
        string newProjects = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(_path, newProjects);
        Console.WriteLine("Arquivo sobrescrito com sucesso!");
    }
}