namespace PixelGrid.Shared.Models.Controller;

public record RequestProjectFileListRequest(long ProjectId);
public record RequestProjectFileListResponse(string[] FilePath);

public record RequestFileLengthRequest(long ProjectId, string FilePath);
public record RequestFileLengthResponse(long FileSize);

public record RequestFileRequest(long ProjectId, string FilePath);
