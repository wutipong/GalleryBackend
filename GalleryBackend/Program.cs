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


app.MapGet("/list", (string path = "") =>
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

app.MapGet("/get/thumbnail/{*path}", ImageHandlers.CreateThumbnail).WithName("Thumbnail");
app.MapGet("/get/image/{*path}", ImageHandlers.CreateViewImage).WithName("View Image");

app.MapGet("/get/file/{*path}", (HttpContext http, string path) =>
{
    var actualPath = new PosixPath(Configurations.BaseDirectory, path);
    var paths = PathUtility.SplitPathAfterArchiveFile(actualPath.ToString());

    if (paths.Length == 1)
    {
        return PhysicalFS.SendFile(paths[0]);
    }

    if (paths.Length == 2)
    {
        return ArchiveFS.SendFile(paths[0], paths[1]);
    }

    throw new InvalidPathException(path, "Nested archive is not supported");
}).WithName("Download");

app.Run();

public record ListResult(string Path, string[] Directories, string[] Archives, string[] Files) { }

