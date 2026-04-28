using LMS.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models.DTOs.Lead;

public class UpdateLeadStatusDto
{
    public int LeadId { get; set; }

    [Required]
    public LeadStatus NewStatus { get; set; }

    [Required(ErrorMessage = "Remark is required")]
    public string Remark { get; set; } = string.Empty;
}
