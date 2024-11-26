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

            foreach (var d in Directory.GetDirectories(path))
            {
                var p = new PosixPath(d);
                directories.AddLast(p.RelativeTo(Configurations.BaseDirectoryPath).ToString());
            }

            foreach (var f in Directory.GetFiles(path))
            {
                var p = new PosixPath(f);
                if (PathUtility.HasArchiveFileExt(f))
                {
                    directories.AddLast(p.RelativeTo(Configurations.BaseDirectoryPath).ToString());
                }

                else
                {
                    files.AddLast(p.RelativeTo(Configurations.BaseDirectoryPath).ToString());
                }
            }

            var output = new ListResult(
                path,
                [.. directories.OrderBy(s => s, (IComparer<string>)StringComparison.OrdinalIgnoreCase.WithNaturalSort())],
                [.. files.OrderBy(s => s, (IComparer<string>)StringComparison.OrdinalIgnoreCase.WithNaturalSort())]
            );

            return output;
        }

        public static Stream ReadFile(String path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
