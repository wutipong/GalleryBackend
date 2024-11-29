namespace Utility.Test
{
    using Utility;

    public class PathUtilityTest
    {
        [Fact]
        public void TestNoArchive()
        {
            var parts = PathUtility.SplitPathAfterArchiveFile("root/somedir/file.jpg");

            Assert.Single(parts);
            Assert.Equal("root/somedir/file.jpg", parts[0]);
        }

        [Fact]
        public void TestOneArchive()
        {
            var parts = PathUtility.SplitPathAfterArchiveFile("root/abc.zip/somedir/file.jpg");

            Assert.Equal(["root/abc.zip", "somedir/file.jpg"], parts);

        }

        [Fact]
        public void TestOneArchiveRoot()
        {
            var parts = PathUtility.SplitPathAfterArchiveFile("root/abc.zip");

            Assert.Equal(["root/abc.zip", ""], parts);

        }

        [Fact]
        public void TestTwoArchive()
        {
            var parts = PathUtility.SplitPathAfterArchiveFile("root/abc.zip/somedir/another.zip/somedir2/somedir3/file.jpg");

            Assert.Equal(["root/abc.zip", "somedir/another.zip", "somedir2/somedir3/file.jpg"], parts);

        }
    }
}
