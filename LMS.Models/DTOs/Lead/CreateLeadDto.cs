using System.ComponentModel.DataAnnotations;

namespace LMS.Models.DTOs.Lead;

public class CreateLeadDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please select an agent")]
    public int AssignedAgentId { get; set; }  // Manager will select
}