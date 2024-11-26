using GalleryBackend;
using NaturalSort.Extension;
using System.Collections.Generic;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives.Rar;
using System;
using PathLib;
using Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/list", (String path = "") =>
{
    var paths = PathUtility.SplitPathAfterArchiveFile(path);
 
    if (paths.Length == 1)
    {
        return PhysicalFS.List(path);
    }

    if (paths.Length == 2)
    {
        return ArchiveFS.List(paths[0], paths[1]);
    }

    throw new InvalidPathException(path, "Nested archive is not supported");
}).WithName("List");

app.MapGet("/thumbnail", ImageHandlers.CreateThumbnail).WithName("Thumbnail");
app.MapGet("/view", ImageHandlers.CreateViewImage).WithName("View Image");

app.MapGet("/get", (String path) =>
{
    var actualPath = Path.Combine(Configurations.BaseDirectory, path);
    var paths = PathUtility.SplitPathAfterArchiveFile(actualPath);

    if (paths.Length == 1)
    {
        using var stream = PhysicalFS.ReadFile(actualPath);
        return Results.Stream(stream);
    }

    if (paths.Length == 2)
    {
        return null;
    }

    throw new InvalidPathException(path, "Nested archive is not supported");
});

app.Run();

public record ListResult(String Path, String[] Directories, String[] Archives, String[] Files) { }

