using System.Text.Json;
using System.Text.Json.Nodes;
using Portfolio.API.Models;

namespace Portfolio.API.Services;

public class JsonPersistenceService : IHostedService
{
    private readonly string _storagePath = Path.Combine(Environment.CurrentDirectory, "Storage");
    private readonly Dictionary<string, string> _archivesPaths;
    private readonly BackedStateService _backedState;
    private Action<IReadOnlyList<ProjectItem>>? _handler;

    public JsonPersistenceService(BackedStateService backedState)
    {
        _backedState = backedState;
        _archivesPaths = new Dictionary<string, string>()
        {
            {"projects", Path.Combine(_storagePath, "projects.json")},
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);

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
        var path = _archivesPaths["projects"];
        if (!File.Exists(path)) return;

        var jsonString = File.ReadAllText(path);
        if (string.IsNullOrEmpty(jsonString)) return;

        var projects = JsonSerializer.Deserialize<IEnumerable<ProjectItem>>(jsonString);
        if (projects is null) throw new FileLoadException("Erro na Desserialização de Lista de Projetos!");

        var snapshot = projects?.ToList().AsReadOnly();
        _backedState.SetProjects(snapshot!);
    }


    public void OverrideProjects(IReadOnlyList<ProjectItem> projects)
    {
        var path = _archivesPaths["projects"];
        var tmpPath = path + ".tmp";

        string json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(tmpPath, json);

        File.Move(tmpPath, path, overwrite: true);
    }
}