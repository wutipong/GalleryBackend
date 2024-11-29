using PathLib;

namespace GalleryBackend
{
    public static class PhysicalFS
    {
        public static ListResult List(string path, SortField sort = SortField.Name,
            Order order = Order.Ascending)
        {
            var directories = new LinkedList<ListObject>();
            var files = new LinkedList<ListObject>();
            var archives = new LinkedList<ListObject>();

            var actualPath = new PosixPath(Configurations.BaseDirectory, path);

            foreach (var d in Directory.GetDirectories(actualPath.ToString()))
            {
                var p = new PosixPath(d);
                directories.AddLast(
                    new ListObject(Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                        DateTime: Directory.GetLastWriteTime(p.ToString())
                    ));
            }

            foreach (var f in Directory.GetFiles(actualPath.ToString()))
            {
                var p = new PosixPath(f);
                if (PathUtility.HasArchiveFileExt(f))
                {
                    archives.AddLast(
                        new ListObject(Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                            DateTime: File.GetLastWriteTime(p.ToString())
                     ));
                }

                else
                {
                    var mimetype = MimeTypes.GetMimeType(p.Filename);
                    if (mimetype.StartsWith("image/") ||
                        mimetype.StartsWith("video/") ||
                        mimetype.StartsWith("audio/"))
                    {
                        files.AddLast(
                            new ListObject(Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                                DateTime: File.GetLastWriteTime(p.ToString())
                        ));
                    }
                }
            }

            var output = ListResult.CreateSorted(
                path,
                directories,
                archives,
                files,
                sort,
                order
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
