namespace LMS.Models.DTOs.LeadRemark;

public class LeadRemarkDto
{
    public int Id { get; set; }

    public string Remark { get; set; } = string.Empty;

    public string OldStatus { get; set; } = string.Empty;

    public string NewStatus { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string ChangedByName { get; set; } = string.Empty;
}
