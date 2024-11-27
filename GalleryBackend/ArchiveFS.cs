using NaturalSort.Extension;
using PathLib;
using SharpCompress.Archives;
using System.IO;

namespace GalleryBackend
{
    public class ArchiveFS
    {
        public static ListResult List(String archivePath, String entryDirPath)
        {
            var actualPath = new PosixPath(Configurations.BaseDirectory, archivePath);
            using var archive = ArchiveFactory.Open(actualPath.ToString());

            var directorySet = new HashSet<string>();
            var files = new LinkedList<string>();

            foreach (var e in archive.Entries)
            {
                var entryPathObj = new PosixPath(e.Key);

                if (entryPathObj.Directory == entryDirPath)
                {
                    if (e.Key?.EndsWith('/') == true)
                    {
                        directorySet.Add(archivePath + "/" + entryPathObj.ToString());
                    }
                    else
                    {
                        var mimetype = MimeTypes.GetMimeType(entryPathObj.ToString());

                        if (mimetype.StartsWith("image/") || mimetype.StartsWith("video/") || mimetype.StartsWith("audio/"))
                        {
                            files.AddLast(archivePath + "/" + entryPathObj.ToString());
                        }
                    }
                }
            }

            return new ListResult(
                Path: string.Join('/', archivePath, entryDirPath),
                Directories: [.. directorySet.ToArray().OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())],
                Archives: [],
                Files: [.. files.OrderBy(s => s, StringComparison.OrdinalIgnoreCase.WithNaturalSort())]
            );
        }

        public static Stream ReadFile(String archivePath, String entryPath)
        {
            using var archive = ArchiveFactory.Open(archivePath);

            var entry = archive.Entries.First((e) => e.Key == entryPath) ?? throw new Exception("entry not found");

            var stream = entry.OpenEntryStream();
            var outstream = new MemoryStream();

            stream.CopyTo(outstream);
            outstream.Position = 0;

            return outstream;
        }

        public static IResult SendFile(String archivePath, String entryPath)
        {
            var steam = ReadFile(archivePath, entryPath);

            return Results.Stream(steam, fileDownloadName: new PosixPath(entryPath).Filename, enableRangeProcessing: true);
        }
    }
}
