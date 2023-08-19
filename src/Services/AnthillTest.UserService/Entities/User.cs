using System.ComponentModel.DataAnnotations;

namespace AnthillTest.UserService.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string? NickName { get; set; }
    public string? Email { get; set; }
    public string? Comments { get; set; }
    public DateTime CreateDate { get; set; }
}
