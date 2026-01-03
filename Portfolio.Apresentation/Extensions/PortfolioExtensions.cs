using System.Buffers.Text;
using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Models;
using Portfolio.API.Services;

namespace Portfolio.API.Extensions;

public static class PortfolioExtensions
{ 
    public static void AddPortfolioExtensions(this WebApplication app)
    {
        var group = app.MapGroup("portfolio");

        group.MapGet("", ([FromServices] BackedStateService jsonBackedStateService) =>
        {
            var projects = jsonBackedStateService.GetProjects();

            if (projects == null) return Results.Empty;

            return Results.Ok(projects); 
        });

        group.MapPost("/override", ([FromServices] BackedStateService jsonBackedStateService, [FromBody] IEnumerable<ProjectItem> projectItem) =>
        {
            
            foreach (var project in projectItem)
            {
                foreach (var image in project.Images)
                {
                    if(Base64.IsValid(image))
                    {
                        var base64 = image;
                        var path = $"{DateTime.Now}{project.Title}.jpeg";
                        //...
                    }
                }
            }
            jsonBackedStateService.SaveProjects(projectItem);
        }).RequireAuthorization();
    }
}