using PixelGrid.Api.Data.enums;

namespace PixelGrid.Api.Data;

public class Client(string name, string ownerId, bool @public = false) : ShareableResource(ownerId)
{
    public string Token { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = name;

    public DateTime? LastConnected { get; set; }
    public DateTime? LastRender { get; set; }

    public bool Public { get; set; } = @public;

    public CyclesDevice[] CyclesDevices { get; set; } = { CyclesDevice.CPU };
}