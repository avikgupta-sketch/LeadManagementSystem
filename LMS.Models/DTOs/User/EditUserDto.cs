using System.ComponentModel.DataAnnotations;

namespace LMS.Models.DTOs.User;

public class EditUserDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    public string Email { get; set; } = string.Empty;
}
