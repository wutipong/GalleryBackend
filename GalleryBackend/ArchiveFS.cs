﻿using PathLib;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;

namespace GalleryBackend
{
    public class ArchiveFS
    {
        public static ListResult List(
            PosixPath physicalPath,
            PosixPath archivePath,
            SortField sort = SortField.Name,
            Order order = Order.Ascending)
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
                        var mimetype = MimeTypes.GetMimeType(entryPath.ToString());

                        if (mimetype.StartsWith("image/") ||
                            mimetype.StartsWith("video/") ||
                            mimetype.StartsWith("audio/"))
                        {
                            files.AddLast(new ListObject(
                                Name: physicalPath.Join(entryPath).ToString(),
                                DateTime: e.LastModifiedTime ?? DateTime.UnixEpoch
                            ));
                        }
                    }
                }
            }

            return ListResult.CreateSorted(
                path: physicalPath.Join(archivePath).ToString(),
                directories: directorySet,
                archives: [],
                files,
                sort, 
                order
            );
        }

        public static Stream ReadFile(string archivePath, string entryPath)
        {
            var pathObj = new PosixPath(archivePath);
            using IArchive archive = OpenArchive(pathObj);

            var entry = archive.Entries.First((e) => e.Key == entryPath) ??
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

        public static IResult SendFile(string archivePath, string entryPath)
        {
            var steam = ReadFile(archivePath, entryPath);

            return Results.Stream(steam, enableRangeProcessing: true);
        }
    }
}
