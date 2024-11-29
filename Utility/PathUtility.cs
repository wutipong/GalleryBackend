namespace Utility
{
    public static class PathUtility
    {
        public static string[] SplitPathAfterArchiveFile(string path)
        {
            var parts = path.Split('/');
            int start = 0;

            var output = new List<string>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (HasArchiveFileExt(parts[i]))
                {
                    output.Add(string.Join('/', parts[start..(i + 1)]));
                    start = i + 1;
                }
            }

            output.Add(string.Join('/', parts[start..]));

            return [.. output];
        }

        public static bool HasArchiveFileExt(string path)
        {
            return path.EndsWith(".zip") ||
                path.EndsWith(".cbz") ||
                path.EndsWith(".rar") ||
                path.EndsWith(".cbr") ||
                path.EndsWith(".7z");
        }

    }
}
