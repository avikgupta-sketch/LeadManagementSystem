using LMS.Models.DTOs.Dashboard;
using MediatR;


public class GetManagerDashboardQuery : IRequest<DashboardDto>
{
    public int ManagerId { get; set; }

    public GetManagerDashboardQuery(int managerId)
    {
        ManagerId = managerId;
    }
}
