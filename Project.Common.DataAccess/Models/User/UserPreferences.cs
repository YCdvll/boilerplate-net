using System.ComponentModel.DataAnnotations;

namespace Project.Common.DataAccess.Models.User;

public class UserPreferences
{
    [Key] public int Id { get; set; }

    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public bool NewsLetter { get; set; }
    public bool EmailNotification { get; set; }
    public bool PushNotification { get; set; }
    public bool VisibleInRegion { get; set; }
}