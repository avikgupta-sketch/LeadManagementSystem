using LMS.Data.Context;
using LMS.Handlers.Users.Queries;
using LMS.Models.DTOs.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Handlers.Users.Handlers;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, EditUserDto?>
{
    private readonly AppDbContext _context;

    public GetUserByIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EditUserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return null;

        return new EditUserDto
        {
            Id = user.Id,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };
    }
}
