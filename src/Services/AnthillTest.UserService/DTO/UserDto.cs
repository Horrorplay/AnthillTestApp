using System.ComponentModel.DataAnnotations;

namespace AnthillTest.UserService.DTO;

public class UserDto
{
    [Display(Name = "NickName")]
    public string? NickName { get; set; }

    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Comments")]
    public string? Comments { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "CreateDate")]
    public DateTime CreateDate { get; set; }
}
