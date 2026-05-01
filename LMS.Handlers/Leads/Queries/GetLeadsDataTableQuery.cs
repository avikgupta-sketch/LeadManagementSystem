using LMS.Models.DTOs.Lead;
using MediatR;

namespace LMS.Handlers.Leads.Queries;

public class GetLeadsDataTableQuery : IRequest<DataTableResponseDto<LeadTableRowDto>>
{
    public DataTableRequestDto Request { get; set; }
    public int UserId { get; set; }
    public bool IsManager { get; set; }

    public GetLeadsDataTableQuery(DataTableRequestDto request, int userId, bool isManager)
    {
        Request = request;
        UserId = userId;
        IsManager = isManager;
    }
}
