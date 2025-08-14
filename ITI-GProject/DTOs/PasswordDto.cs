namespace ITI_GProject.DTOs;

public class UserChangePasswordDto
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
}

public class AdminChangePasswordDto
{
    public required string NewPassword { get; set; }
}