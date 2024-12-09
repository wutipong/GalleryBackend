
namespace GalleryBackend;

public record ListObject(string Name, DateTime DateTime);
public record ListResult(string Path, ListObject[] Directories, ListObject[] Archives, ListObject[] Files);
