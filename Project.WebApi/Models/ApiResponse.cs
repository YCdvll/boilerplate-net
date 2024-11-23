namespace Project.WebApi.Models;

public class ApiResponse
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public object Data { get; set; }
    public int ExpEarned { get; set; }
}