using PathLib;

namespace GalleryBackend
{
    public static class PhysicalFS
    {
        public static ListResult List(PosixPath path, SortField sort = SortField.Name,
            Order order = Order.Ascending)
        {
            var directories = new LinkedList<ListObject>();
            var files = new LinkedList<ListObject>();
            var archives = new LinkedList<ListObject>();

            var actualPath = Configurations.BaseDirectoryPath.Join(path);

            foreach (var p in actualPath.ListDir(SearchOption.TopDirectoryOnly))
            {
                if(p.IsDir()){
                    directories.AddLast(
                        new ListObject(Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                            DateTime: Directory.GetLastWriteTime(p.ToString())
                        ));
                } else
                {
                    if (PathUtility.HasArchiveFileExt(p))
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
            }

            var pathString = path.ToString();
            if (pathString == ".")
            {
                pathString = "";
            }

            var output = ListResult.CreateSorted(
                pathString,
                directories,
                archives,
                files,
                sort,
                order
            );

            return output;
        }

        public static Stream ReadFile(PosixPath path)
        {
            return path.Open(FileMode.Open);
        }

        public static IResult SendFile(PosixPath path)
        {
            return Results.File(path.ToString(), enableRangeProcessing: true);
        }
    }
}
