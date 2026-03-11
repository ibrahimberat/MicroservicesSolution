using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands
{
    public class RegisterCommand : IRequest<TokenDto>
    {
        public RegisterDto RegisterDto { get; set; } = null!;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IAuthRepository _authRepository;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IAuthRepository authRepository)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _authRepository = authRepository;
        }

        public async Task<TokenDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _authRepository.FindByEmailAsync(request.RegisterDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var user = new ApplicationUser
            {
                UserName = request.RegisterDto.Username,
                Email = request.RegisterDto.Email,
                FirstName = request.RegisterDto.FirstName,
                LastName = request.RegisterDto.LastName
            };

            var result = await _userManager.CreateAsync(user, request.RegisterDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "User");

            return await _tokenService.GenerateTokenAsync(user);
        }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.RegisterDto.Username).NotEmpty().MinimumLength(3);
            RuleFor(x => x.RegisterDto.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.RegisterDto.Password).NotEmpty().MinimumLength(6);
        }
    }
}