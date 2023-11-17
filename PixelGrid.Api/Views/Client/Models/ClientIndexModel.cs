namespace PixelGrid.Api.Views.Client.Models;

public record ClientIndexModel(List<Data.Client> OwnClients, List<Data.Client> SharedClients, List<Data.Client> PublicClients);