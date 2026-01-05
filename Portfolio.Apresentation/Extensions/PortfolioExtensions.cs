using System.Buffers.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Models;
using Portfolio.API.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Portfolio.API.Extensions;

public static class PortfolioExtensions
{ 
    public static void AddPortfolioExtensions(this WebApplication app)
    {
        var group = app.MapGroup("portfolio").WithTags("Portfolio");

        group.MapGet("", ([FromServices] BackedStateService jsonBackedStateService) =>
        {
            var projects = jsonBackedStateService.GetProjects();

            if (projects == null) return Results.Empty;

            return Results.Ok(projects); 
        });

        group.MapPost("/override", async ([FromServices] BackedStateService jsonBackedStateService, [FromBody] IEnumerable<ProjectItem> projectItem) =>
        {
            foreach (var project in projectItem)
            {
                var images = project.Images.ToList();
                for (int i = 0; i < images.Count; i++)
                {
                    if (IsBase64(images[i]))
                    {
                        var base64 = images[i];
                        var directoryPath = Path.Combine(Environment.CurrentDirectory, "Storage", "Images");
                        var imageName = $"{Guid.NewGuid()}.jpg";
                        await SaveImage(directoryPath, imageName, base64);

                        images[i] = Path.Combine(directoryPath, imageName);
                    }
                }

                project.Images = images;
            }
            jsonBackedStateService.SaveProjects(projectItem);
        }).RequireAuthorization();
    }

    private static async Task SaveImage(string directoryPath, string imageName, string base64)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);


        var bytes = Convert.FromBase64String(CleanBase64(base64));
        await File.WriteAllBytesAsync(Path.Combine(directoryPath, imageName), bytes);
    }

    // Helpers
    private static string CleanBase64(string input)
    {
        var match = Regex.Match(input, @"^data:image\/\w+;base64,(.+)$");
        return match.Success ? match.Groups[1].Value : input;
    }

    private static bool IsBase64(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var match = Regex.Match(input, @"^data:image\/\w+;base64,(.+)$");
        if (match.Success) input = match.Groups[1].Value;

        try
        {
            Convert.FromBase64String(input);
            return true;
        }
        catch
        {
            return false;
        }
    }
}