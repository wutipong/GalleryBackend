namespace GalleryBackend
{
    public class ArchiveFS
    {
        public static ListResult List(String archivePath, String entryPath)
        {
            return new ListResult(String.Join('/', archivePath, entryPath), [], []);
        }
    }
}
