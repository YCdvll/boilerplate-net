using System.ComponentModel;

namespace Project.Common.Services.Models;

public enum ContainerName
{
    [Description("gpx")] Gpx = 1,
    [Description("images")] Images = 2,
    [Description("public")] Public = 3
}