using Microsoft.Net.Http.Headers;
using PathLib;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using System.IO;

namespace GalleryBackend
{
    public class ArchiveFS
    {
        public static ListResult List(
            PosixPath physicalPath,
            PosixPath archivePath)
        {
            var actualPath = Configurations.BaseDirectoryPath.Join(physicalPath);
            using var archive = OpenArchive(actualPath);

            var directorySet = new HashSet<ListObject>();
            var files = new LinkedList<ListObject>();

            var archivePathStr = archivePath.ToString();
            if (archivePathStr == ".")
            {
                archivePathStr = "";
            }

            foreach (var e in archive.Entries)
            {
                var entryPath = new PosixPath(e.Key);

                if (entryPath.Directory == archivePathStr)
                {
                    if (e.IsDirectory)
                    {
                        directorySet.Add(new ListObject(
                            Name: physicalPath.Join(entryPath).ToString(),
                            DateTime: e.LastModifiedTime ?? DateTime.UnixEpoch
                        ));
                    }
                    else
                    {
                        var mimetype = MimeTypes.GetMimeType(entryPath.Filename);

                        if (mimetype.StartsWith("image/") ||
                            mimetype.StartsWith("video/") ||
                            mimetype.StartsWith("audio/") ||
                            mimetype == "application/pdf")
                        {
                            files.AddLast(new ListObject(
                                Name: physicalPath.Join(entryPath).ToString(),
                                DateTime: e.LastModifiedTime ?? DateTime.UnixEpoch
                            ));
                        }
                    }
                }
            }

            return new ListResult(
                Path: physicalPath.Join(archivePath).ToString(),
                Directories: directorySet,
                Archives: [],
                Files: files
            );
        }

        public static Stream ReadFile(PosixPath physicalPath, PosixPath archivePath)
        {
            using IArchive archive = OpenArchive(Configurations.BaseDirectoryPath.Join(physicalPath));

            var entry = archive.Entries.First((e) => e.Key == archivePath.ToString()) ??
                throw new Exception("entry not found");

            var stream = entry.OpenEntryStream();
            var outstream = new MemoryStream();

            stream.CopyTo(outstream);
            outstream.Position = 0;

            return outstream;
        }

        private static IArchive OpenArchive(PosixPath path)
        {
            return path.Extension switch
            {
                ".cbz" => ZipArchive.Open(path.ToString()),
                ".cbr" => RarArchive.Open(path.ToString()),
                _ => ArchiveFactory.Open(path.ToString())
            };
        }

        public static IResult SendFile(HttpContext http, PosixPath archivePath, PosixPath entryPath)
        {
            var disposition = new ContentDispositionHeaderValue(dispositionType: "inline");
            disposition.SetHttpFileName(entryPath.Filename);

            http.Response.Headers.ContentDisposition = disposition.ToString();

            var steam = ReadFile(archivePath, entryPath);

            return Results.Stream(steam,
                enableRangeProcessing: true,
                contentType: MimeTypes.GetMimeType(entryPath.Filename));
        }
    }
}
