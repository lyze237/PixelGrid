using PixelGrid.Shared.Models.Controller;
using Refit;

namespace PixelGrid.Client.Services;

public interface IApiClient
{
    [Get("/Api/Files/RequestProjectFileList")]
    Task<IApiResponse<RequestProjectFileListResponse>> RequestProjectFileList([Query] RequestProjectFileListRequest request);

    [Get("/Api/Files/RequestFile")]
    Task<HttpContent> RequestFile([Query] RequestFileRequest request);
}