using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Models;

namespace Portfolio.API.Extensions;

public static class PortfolioExtensions
{ 
    public static void AddPortfolioExtensions(this WebApplication app)
    {
        var group = app.MapGroup("portfolio");

        group.MapGet("/",([FromBody] ProjectItem projectItem) =>
        {

        });
    }
}