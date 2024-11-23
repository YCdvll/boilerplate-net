using System.ComponentModel.DataAnnotations;

namespace Project.Common.DataAccess.Models;

public class SqlRepository
{
    [Key] public int Id { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
}