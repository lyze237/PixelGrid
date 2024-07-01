using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using PixelGrid.Shared.Renderer.Options;

namespace PixelGrid.Server.Domain.Entities;

public class ProjectEntity
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    public string Permission { get; set; }

    public string RenderFilePath { get; set; }
    public string LogFilePath { get; set; }
    
    public WorkspaceEntity Workspace { get; set; }
    
    public string OptionsContent { get; set; }
    public string OptionsClass { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    [NotMapped]
    public RenderOptions Options
    {
        get => (RenderOptions) JsonSerializer.Deserialize(OptionsContent, Type.GetType(OptionsClass));
        set
        {
            OptionsContent = JsonSerializer.Serialize(value);
            OptionsClass = value.GetType().FullName;
        }
    }
}