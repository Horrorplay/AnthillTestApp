using AnthillTest.AuthService.Entities.Enums;

namespace AnthillTest.AuthService.Entities;

public class Application : BaseEntity
{
    public AppUserStatus Status { get; set; } = AppUserStatus.Created;
    public long? EmployeeId { get; set; }
    public AppUser? User { get; set; }
}