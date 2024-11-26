using GalleryBackend;
using NaturalSort.Extension;


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
    var actualPath = Path.Combine(Configurations.BaseDirectory, path);
    var directories = Directory.GetDirectories(actualPath);
    var files = Directory.GetFiles(actualPath);

    for (int i = 0; i < files.Length; i++)
    {
        files[i] = files[i][(Configurations.BaseDirectory.Length+1)..];
    }

    for (int i = 0; i < directories.Length; i++)
    {
        directories[i] = directories[i][(Configurations.BaseDirectory.Length +1)..] +"/";
    }

    var output = new ListResult(
        path,
        [.. directories.OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())],
        [.. files.OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())]
    );

    return output;
}).WithName("List");

app.MapGet("/thumbnail", ImageHandlers.CreateThumbnail).WithName("Thumbnail");
app.MapGet("/view", ImageHandlers.CreateViewImage).WithName("View Image");

app.Run();

internal record ListResult(String Path, String[] Directories, String[] files) { }