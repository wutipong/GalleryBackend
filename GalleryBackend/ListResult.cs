
namespace GalleryBackend;

public record ListObject(string Name, DateTime DateTime);
public record ListResult(string Path, IEnumerable<ListObject> Directories, IEnumerable<ListObject> Archives, IEnumerable<ListObject> Files);
