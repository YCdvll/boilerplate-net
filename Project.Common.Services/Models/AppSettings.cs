namespace Project.Common.Services.Models;

public class AppSettings
{
    public string ApiDomain { get; set; }
    public string AllowOrigin { get; set; }
    public string FrontDomain { get; set; }
    public string Secret { get; set; }
    public string EmailUser { get; set; }
    public string EmailSecret { get; set; }
    public string EmailSmtp { get; set; }
    public bool IsDebug { get; set; }
}