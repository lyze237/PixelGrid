﻿using Microsoft.AspNetCore.Identity;

namespace PixelGrid.Api.Data;

public class User : IdentityUser
{
    public virtual List<Client> OwnedClients { get; set; } = new();
    public virtual List<Project> OwnedProjects { get; set; } = new();
    public virtual List<Client> SharedClients { get; set; } = new();
    public virtual List<Project> SharedProjects { get; set; } = new();
}