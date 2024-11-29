namespace GalleryBackend.Test
{
    using GalleryBackend;
    using PathLib;

    public class PathUtilityTest
    {
        [Fact]
        public void TestNoArchive()
        {
            var ( physicalPath,  archivePath,  hasArchivePath) 
                = PathUtility.SplitPathAfterArchiveFile(new PosixPath("root/somedir/file.jpg"));

            Assert.False(hasArchivePath);
            Assert.Equal("root/somedir/file.jpg", physicalPath.ToString());
            Assert.Equal(".", archivePath.ToString());
        }

        [Fact]
        public void TestOneArchive()
        {
            var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile( new PosixPath("root/abc.zip/somedir/file.jpg"));

            Assert.True(hasArchivePath);
            Assert.Equal("root/abc.zip", physicalPath.ToString());
            Assert.Equal("somedir/file.jpg", archivePath.ToString());
        }

        [Fact]
        public void TestOneArchiveRoot()
        {
            var (physicalPath, archivePath, hasArchivePath)
                = PathUtility.SplitPathAfterArchiveFile(new PosixPath("root/abc.zip"));

            Assert.True(hasArchivePath);
            Assert.Equal("root/abc.zip", physicalPath.ToString());
            Assert.Equal(".", archivePath.ToString());

        }
    }
}
