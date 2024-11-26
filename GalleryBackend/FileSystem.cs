namespace GalleryBackend
{
    public record VirtualPath(String FSPath, String ArchivePath);

    public static class FileSystem
    {
        public static VirtualPath CreateVirtualPath(String actualPath)
        {
            var parts = actualPath.Split('/');

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].EndsWith(".zip") || parts[i].EndsWith(".rar"))
                {
                    var subpath = Path.Combine(parts[..i]);

                    return new VirtualPath(subpath, Path.Combine(parts[i..]));
                }
            }

            return new VirtualPath(actualPath, "");
        }
    }
}
