using NaturalSort.Extension;
using PathLib;
using Utility;

namespace GalleryBackend
{
    public static class PhysicalFS
    {
        public static ListResult List(string path)
        {
            var directories = new LinkedList<string>();
            var files = new LinkedList<string>();
            var archives = new LinkedList<string>();

            var actualPath = new PosixPath(Configurations.BaseDirectory, path);

            foreach (var d in Directory.GetDirectories(actualPath.ToString()))
            {
                var p = new PosixPath(d);
                directories.AddLast(p.RelativeTo(Configurations.BaseDirectoryPath).ToString());
            }

            foreach (var f in Directory.GetFiles(actualPath.ToString()))
            {
                var p = new PosixPath(f);
                if (PathUtility.HasArchiveFileExt(f))
                {
                    archives.AddLast(p.RelativeTo(Configurations.BaseDirectoryPath).ToString());
                }

                else
                {
                    var mimetype = MimeTypes.GetMimeType(p.Filename);
                    if (mimetype.StartsWith("image/") || 
                        mimetype.StartsWith("video/") ||
                        mimetype.StartsWith("audio/"))
                    {
                        files.AddLast(p.RelativeTo(Configurations.BaseDirectoryPath).ToString());
                    }
                }
            }

            var output = new ListResult(
                Path: path,
                Directories: [.. directories.OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())],
                Archives: [.. archives.OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())],
                Files: [.. files.OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())]
            );

            return output;
        }

        public static Stream ReadFile(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public static IResult SendFile(string path)
        {
            return Results.File(path, enableRangeProcessing: true);
        }
    }
}
