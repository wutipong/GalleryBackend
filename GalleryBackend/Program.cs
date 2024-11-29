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


app.MapGet("/list", (string path = "", string sortby = "name", string order = "ascending") =>
{
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

    var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile(new PosixPath(path));

    if (hasArchivePath)
    {
        return ArchiveFS.List(physicalPath, archivePath, sortVal, orderVal);
    }
    else
    {
        return PhysicalFS.List(physicalPath, sortVal, orderVal);
    }
}).WithName("List");

app.MapGet("/get/thumbnail/{*path}", ImageHandlers.CreateThumbnail).WithName("Thumbnail");
app.MapGet("/get/image/{*path}", ImageHandlers.CreateViewImage).WithName("View Image");

app.MapGet("/get/file/{*path}", (HttpContext http, string path) =>
{
    var actualPath = new PosixPath(Configurations.BaseDirectory, path);
    var paths = PathUtility.SplitPathAfterArchiveFile(actualPath.ToString());

    var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile(actualPath);

    if (hasArchivePath)
    {
        return ArchiveFS.SendFile(physicalPath.ToString(), archivePath.ToString());
    }
    else
    {
        return PhysicalFS.SendFile(physicalPath.ToString());
    }
}).WithName("Download");

app.Run();

