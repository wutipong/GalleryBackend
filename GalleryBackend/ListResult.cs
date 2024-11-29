
using NaturalSort.Extension;

namespace GalleryBackend;

public enum Order
{
    Ascending, Descending
}

public enum SortField
{
    Name, DateTime
}
public record ListObject(string Name, DateTime DateTime);
public record ListResult(string Path, ListObject[] Directories, ListObject[] Archives, ListObject[] Files)
{
    public static ListResult CreateSorted(
        string path,
        IEnumerable<ListObject> directories,
        IEnumerable<ListObject> archives,
        IEnumerable<ListObject> files,
        SortField field = SortField.Name,
        Order order = Order.Ascending)
    {
        directories = field switch
        {
            SortField.Name => order switch
            {
                Order.Ascending => directories.OrderBy(
                    s => s.Name,
                    StringComparison.OrdinalIgnoreCase.WithNaturalSort()),
                Order.Descending => directories.OrderByDescending(
                    s => s.Name,
                    StringComparison.OrdinalIgnoreCase.WithNaturalSort()),
                _ => throw new NotImplementedException(),
            },

            SortField.DateTime => order switch
            {
                Order.Ascending => directories.OrderBy(s => s.DateTime),
                Order.Descending => directories.OrderByDescending(s => s.DateTime),
                _ => throw new NotImplementedException(),
            },
            _ => throw new NotImplementedException()
        };
        archives = field switch
        {
            SortField.Name => order switch
            {
                Order.Ascending => archives.OrderBy(
                    s => s.Name,
                    StringComparison.OrdinalIgnoreCase.WithNaturalSort()),

                Order.Descending => archives.OrderByDescending(
                    s => s.Name,
                    StringComparison.OrdinalIgnoreCase.WithNaturalSort()),
                _ => throw new NotImplementedException(),
            },

            SortField.DateTime => order switch
            {
                Order.Ascending => archives.OrderBy(s => s.DateTime),
                Order.Descending => archives.OrderByDescending(s => s.DateTime),
                _ => throw new NotImplementedException(),
            },
            _ => throw new NotImplementedException()
        };
        files = field switch
        {
            SortField.Name => order switch
            {
                Order.Ascending => files.OrderBy(
                    s => s.Name,
                    StringComparison.OrdinalIgnoreCase.WithNaturalSort()),
                Order.Descending => files.OrderByDescending(
                    s => s.Name,
                    StringComparison.OrdinalIgnoreCase.WithNaturalSort()),
                _ => throw new NotImplementedException(),
            },

            SortField.DateTime => order switch
            {
                Order.Ascending => files.OrderBy(s => s.DateTime),
                Order.Descending => files.OrderByDescending(s => s.DateTime),
                _ => throw new NotImplementedException(),
            },
            _ => throw new NotImplementedException()
        };

        return new ListResult(
            Path: path,
            Directories: [.. directories],
            Archives: [.. archives],
            Files: [.. files]);
    }

    public static ListResult CreateSorted(
        ListResult l,
        SortField sort = SortField.Name,
        Order order = Order.Ascending)
    {
        return CreateSorted(l.Path, l.Directories, l.Archives, l.Files, sort, order);
    }
}

