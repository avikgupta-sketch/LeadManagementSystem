using LMS.Models.DTOs.Auth;
using MediatR;

namespace LMS.Handlers.Auth.Commands;

public class LoginCommand : IRequest<bool>
{
    public LoginDto LoginDto { get; set; }

    public LoginCommand(LoginDto dto)
    {
        LoginDto = dto;
    }
}