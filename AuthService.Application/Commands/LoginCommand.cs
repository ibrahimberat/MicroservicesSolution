using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class LoginCommand : IRequest<TokenDto>
    {
        public LoginDto LoginDto { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IAuthRepository authRepository, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = !string.IsNullOrEmpty(request.LoginDto.Username)
                ? await _authRepository.FindByNameAsync(request.LoginDto.Username)
                : await _authRepository.FindByEmailAsync(request.LoginDto.Email!);

            if (user == null || !await _authRepository.CheckPasswordAsync(user, request.LoginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return await _tokenService.GenerateTokenAsync(user);
        }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.LoginDto.Password).NotEmpty().MinimumLength(6);

            When(x => string.IsNullOrEmpty(x.LoginDto.Username) && string.IsNullOrEmpty(x.LoginDto.Email), () =>
            {
                RuleFor(x => x.LoginDto.Username).NotEmpty().WithMessage("Username or Email is required");
            });
        }
    }
}