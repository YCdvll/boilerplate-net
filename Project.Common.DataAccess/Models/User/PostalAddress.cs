using System.ComponentModel.DataAnnotations;

namespace Project.Common.DataAccess.Models.User;

public class PostalAddress
{
    [Key] public int Id { get; set; }

    public int UserId { get; set; }
    public string Address { get; set; }
    public string PostCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}