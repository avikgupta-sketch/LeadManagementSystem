namespace LMS.Models.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalAgents { get; set; }   // 🔥 used by Manager dashboard

    public int TotalLeads { get; set; }

    public int New { get; set; }
    public int InProgress { get; set; }
    public int FollowUp { get; set; }
    public int Interested { get; set; }
    public int NotInterested { get; set; }
    public int Converted { get; set; }
    public int Closed { get; set; }
    public int Rejected { get; set; }

    // Aggregate "lost / not converted" — Rejected + Closed + NotInterested.
    public int Lost => Rejected + Closed + NotInterested;

    // Aggregate "open / in-pipeline" — anything not yet finalized.
    public int Open => New + InProgress + FollowUp + Interested;
}
