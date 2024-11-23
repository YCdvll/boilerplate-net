using System.ComponentModel.DataAnnotations;

namespace Project.Common.DataAccess.Models;

public class Role
{
    [Key] public int Id { get; set; }

    public string Name { get; set; }
}