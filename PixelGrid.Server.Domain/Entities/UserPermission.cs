namespace PixelGrid.Server.Domain.Entities;

[Flags]
public enum UserPermission
{
    Render = 1,
    Manage = 2, 
    Admin = 4
}