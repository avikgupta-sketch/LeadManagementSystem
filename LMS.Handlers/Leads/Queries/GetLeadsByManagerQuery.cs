using MediatR;
using LMS.Models.Entities;

namespace LMS.Handlers.Leads.Queries;

public class GetLeadsByManagerQuery : IRequest<List<Lead>>
{
    public int ManagerId { get; set; }

    public GetLeadsByManagerQuery(int managerId)
    {
        ManagerId = managerId;
    }
}
