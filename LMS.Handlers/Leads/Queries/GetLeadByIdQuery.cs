using MediatR;
using LMS.Models.Entities;
using LMS.Models.DTOs.Lead;

namespace LMS.Handlers.Leads.Queries;

public class GetLeadByIdQuery : IRequest<LeadDetailDto?>
{
    public int LeadId { get; set; }

    public GetLeadByIdQuery(int leadId)
    {
        LeadId = leadId;
    }
}