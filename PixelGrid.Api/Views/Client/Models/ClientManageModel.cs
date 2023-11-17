using PixelGrid.Api.Data;

namespace PixelGrid.Api.Views.Client.Models;

public record ClientManageModel(Data.Client Client, List<User> Users);