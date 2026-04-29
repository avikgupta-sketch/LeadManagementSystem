using System.ComponentModel.DataAnnotations;

namespace LMS.Models.DTOs.Lead;

public class EditLeadDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;
}
