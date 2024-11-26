using NaturalSort.Extension;
using PathLib;
using Utility;

namespace GalleryBackend
{
    public static class PhysicalFS
    {
        public static ListResult List(String path)
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
                    if (MimeTypes.GetMimeType(p.Filename).StartsWith("image/"))
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

        public static Stream ReadFile(String path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
