using LMS.Models.DTOs.LeadRemark;
using LMS.Models.Enums;

namespace LMS.Models.DTOs.Lead;

public class LeadDetailDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Customer info
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Other;

    // Status
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public bool IsTerminal =>
        Status == LeadStatus.Converted
        || Status == LeadStatus.Closed
        || Status == LeadStatus.Rejected;

    // Assigned agent (used by Manager's Reassign panel)
    public int AssignedAgentId { get; set; }
    public string AssignedAgentName { get; set; } = string.Empty;
    public bool AssignedAgentDeleted { get; set; }

    public List<LeadRemarkDto> Remarks { get; set; } = new();
}
