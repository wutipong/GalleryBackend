using PathLib;

namespace GalleryBackend
{
    public static class PhysicalFS
    {
        public static ListResult List(PosixPath path)
        {
            var directories = new LinkedList<ListObject>();
            var files = new LinkedList<ListObject>();
            var archives = new LinkedList<ListObject>();

            var actualPath = Configurations.BaseDirectoryPath.Join(path);

            foreach (var p in actualPath.ListDir(SearchOption.TopDirectoryOnly))
            {
                if (p.IsDir())
                {
                    directories.AddLast(
                        new ListObject(
                            Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                            DateTime: p.DirectoryInfo.LastWriteTime
                        ));
                }
                else
                {
                    if (PathUtility.HasArchiveFileExt(p))
                    {
                        archives.AddLast(
                            new ListObject(
                                Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                                DateTime: p.FileInfo.LastWriteTime
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
                                new ListObject(
                                    Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                                    DateTime: p.FileInfo.LastWriteTime
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

            var output = new ListResult(
                Path: pathString,
                Directories: directories,
                Archives: archives,
                Files: files
            );

            return output;
        }

        public static Stream ReadFile(PosixPath path)
        {
            return Configurations.BaseDirectoryPath.Join(path).FileInfo.OpenRead();
        }

        public static IResult SendFile(PosixPath path)
        {
            return Results.File(
                Configurations.BaseDirectoryPath.Join(path).ToString(),
                enableRangeProcessing: true);
        }
    }
}
