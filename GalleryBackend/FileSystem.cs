namespace GalleryBackend
{
    using Utility;
    public record VirtualPath(string FSPath, string ArchivePath);

    public static class FileSystem
    {
        public static VirtualPath CreateVirtualPath(string actualPath)
        {
            var parts = actualPath.Split('/');

            for (int i = 0; i < parts.Length; i++)
            {
                if (PathUtility.HasArchiveFileExt(parts[i]))
                {
                    var subpath = Path.Combine(parts[..i]);

                    return new VirtualPath(subpath, Path.Combine(parts[i..]));
                }
            }

            return new VirtualPath(actualPath, "");
        }
    }
}
