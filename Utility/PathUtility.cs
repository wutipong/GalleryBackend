using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class PathUtility
    {
        public static string[] SplitPathAfterArchiveFile(String path)
        {

            var parts = path.Split('/');
            int start = 0;

            var output = new List<String>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (HasArchiveFileExt(parts[i]))
                {
                    output.Add(String.Join('/', parts[start..(i + 1)]));
                    start = i + 1;
                }
            }

            output.Add(String.Join('/', parts[start..]));

            return [.. output];
        }

        public static bool HasArchiveFileExt(String path)
        {
            return path.EndsWith(".zip") || path.EndsWith(".rar");
        }

    }
}
