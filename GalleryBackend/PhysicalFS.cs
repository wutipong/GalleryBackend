using Microsoft.Net.Http.Headers;
using PathLib;
using System.Net.Mime;

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
                    else if (PathUtility.IsViewableFile(p))
                    {
                        files.AddLast(
                            new ListObject(
                                Name: p.RelativeTo(Configurations.BaseDirectoryPath).ToString(),
                                DateTime: p.FileInfo.LastWriteTime
                        ));
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

        public static IResult SendFile(HttpContext http, PosixPath path)
        {
            var disposition = new ContentDispositionHeaderValue(dispositionType: "inline");
            disposition.SetHttpFileName(path.Filename);

            http.Response.Headers.ContentDisposition = disposition.ToString();

            return Results.File(
                Configurations.BaseDirectoryPath.Join(path).ToString(),
                contentType: MimeTypes.GetMimeType(path.Filename),
                enableRangeProcessing: true);
        }
    }
}
