using GalleryBackend;
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


app.MapGet("/list", (string path = "", string sortby="name", string order="ascending") =>
{
    var paths = PathUtility.SplitPathAfterArchiveFile(path);

    var sortVal = sortby switch
    {
        "name" => SortField.Name,
        "dateTime" => SortField.DateTime,
        _ => throw new NotImplementedException()
    };

    var orderVal = order switch
    {
        "ascending" => Order.Ascending,
        "descending" => Order.Descending,
        _ => throw new NotImplementedException(),
    };

    if (paths.Length == 1)
    {
        return PhysicalFS.List(path, sortVal, orderVal);
    }

    if (paths.Length == 2)
    {
        return ArchiveFS.List(paths[0], paths[1], sortVal, orderVal);
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

