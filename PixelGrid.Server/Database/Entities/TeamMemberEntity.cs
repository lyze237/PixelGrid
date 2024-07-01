namespace PixelGrid.Server.Database.Entities;

public class TeamMemberEntity
{
    public long Id { get; set; }

    public UserEntity User { get; set; }
    public TeamEntity Team { get; set; }

    public UserPermission Permissions { get; set; }
}