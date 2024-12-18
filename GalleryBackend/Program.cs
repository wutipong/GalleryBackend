using GalleryBackend;
using PathLib;

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
    var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile(new PosixPath(path));

    if (hasArchivePath)
    {
        return ArchiveFS.List(physicalPath, archivePath);
    }
    else
    {
        return PhysicalFS.List(physicalPath);
    }
}).WithName("List");

app.MapGet("/get/list_thumbnail/{*path}", (string path,
    int width = Configurations.ListThumbnailWidth,
    int height = Configurations.ListThumbnailHeight
) => ImageHandlers.CreateListThumbnail(path, width, height)).WithName("Thumbnail for list");

app.MapGet("/get/grid_thumbnail/{*path}",
    (string path) => ImageHandlers.CreateGridThumbnail(path)
).WithName("Thumbnail for grid");

app.MapGet("/get/image/{*path}", 
    ImageHandlers.CreateViewImage
).WithName("View Image");

app.MapGet("/get/file/{*path}", (HttpContext http, string path) =>
{
    var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile(new PosixPath(path));

    if (hasArchivePath)
    {
        return ArchiveFS.SendFile(physicalPath, archivePath);
    }
    else
    {
        return PhysicalFS.SendFile(physicalPath);
    }
}).WithName("Download");

app.Run();

