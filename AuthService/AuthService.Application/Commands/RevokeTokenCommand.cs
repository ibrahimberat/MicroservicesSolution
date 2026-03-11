using MediatR;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Commands
{
    public class RevokeTokenCommand : IRequest<bool>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, bool>
    {
        private readonly ITokenService _tokenService;

        public RevokeTokenCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            return await _tokenService.RevokeTokenAsync(request.RefreshToken);
        }
    }
}
