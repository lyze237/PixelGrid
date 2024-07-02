using PixelGrid.Shared.Models.Controller;
using Refit;

namespace PixelGrid.Client.Services;

public interface IApiClient
{
    [Get("Files/RequestProjectFileList")]
    Task<IApiResponse<RequestProjectFileListResponse>> RequestProjectFileList(RequestProjectFileListRequest request);

    [Get("Files/RequestFile")]
    Task<HttpContent> RequestFile(RequestFileRequest request);
}