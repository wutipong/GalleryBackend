using NetVips;
using PathLib;

namespace GalleryBackend
{
    public enum ThumbnailType
    {
        Grid, List
    }
    public static class ImageHandlers
    {
        private static Stream GetStream(PosixPath path)
        {
            var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile(path);

            if (hasArchivePath)
            {
                return ArchiveFS.ReadFile(physicalPath, archivePath);
            }
            else
            {
                return PhysicalFS.ReadFile(physicalPath);
            }
        }
        public static IResult CreateThumbnail(string path, ThumbnailType type = ThumbnailType.Grid)
        {
            using var stream = GetStream(new PosixPath(path));
            using var image = Image.NewFromStream(stream);

            var scale = image.Width > image.Height ?
                (double)Configurations.ListThumbnailWidth / (double)image.Width :
                (double)Configurations.ListThumbnailHeight / (double)image.Height;

            using var thumb = type switch
            {
                ThumbnailType.Grid => image.ThumbnailImage(
                    width: Configurations.GridThumbnailWidth,
                    height: Configurations.GridThumbnailHeight,
                    crop: Enums.Interesting.Entropy
                ),

                ThumbnailType.List => image.Resize(scale),
                _ => throw new NotImplementedException(),
            };

            var output = thumb.JpegsaveBuffer();

            return Results.Bytes(output, "image/jpeg");
        }

        public static IResult CreateViewImage(string path)
        {
            var pathObj = new PosixPath(path);
            var filename = new PosixPath(path).Filename;

            if (MimeTypes.GetMimeType(filename) == "image/gif")
            {
                return Results.Stream(
                    stream: GetStream(pathObj),
                    fileDownloadName: filename);
            }

            using var stream = GetStream(pathObj);
            using var image = Image.NewFromStream(stream);

            byte[] output;
            if (image.Width < Configurations.ViewImageWidth ||
                image.Height < Configurations.ViewImageHeight)
            {
                output = image.WebpsaveBuffer();
            }
            else
            {
                using var thumb = image.ThumbnailImage(
                    width: Configurations.ViewImageWidth,
                    height: Configurations.ViewImageHeight,
                    crop: Enums.Interesting.None);

                output = thumb.WebpsaveBuffer();
            }

            return Results.Bytes(
                output,
                fileDownloadName: $"{filename}.webp");
        }
    }
}
