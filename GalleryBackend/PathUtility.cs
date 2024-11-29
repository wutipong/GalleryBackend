using PathLib;

namespace GalleryBackend
{
    public static class PathUtility
    {
        public static string[] SplitPathAfterArchiveFile(string path)
        {
            var pathObject = new PosixPath(path);

            foreach (var i in pathObject.Parents())
            {
                Console.WriteLine(i.ToString());
            }

            var parts = pathObject.ToString().Split('/');
            int start = 0;

            var output = new List<string>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (HasArchiveFileExt(parts[i]))
                {
                    output.Add(string.Join('/', parts[start..(i + 1)]));
                    start = i + 1;
                }
            }

            output.Add(string.Join('/', parts[start..]));

            return [.. output];
        }

        public static (PosixPath physicalPath, PosixPath archivePath, bool hasArchivePath)
            SplitPathAfterArchiveFile(this PosixPath path)
        {
            if (HasArchiveFileExt(path))
            {
                return (physicalPath: path, archivePath: new PosixPath(), hasArchivePath: true);
            }

            foreach (var p in path.Parents())
            {
                if (HasArchiveFileExt(p))
                {
                    var physicalPath = p;
                    var archivePath = path.RelativeTo(physicalPath);

                    return (physicalPath, archivePath, true);
                }
            }

            return (path, new PosixPath(), false);
        }

        public static bool HasArchiveFileExt(string path)
        {
            return path.EndsWith(".zip") ||
                path.EndsWith(".cbz") ||
                path.EndsWith(".rar") ||
                path.EndsWith(".cbr") ||
                path.EndsWith(".7z");
        }

        public static bool HasArchiveFileExt(PosixPath path)
        {
            return path.Extension == ".zip" ||
                path.Extension == ".cbz" ||
                path.Extension == ".rar" ||
                path.Extension == ".cbr" ||
                path.Extension == ".7z";
        }

    }
}
