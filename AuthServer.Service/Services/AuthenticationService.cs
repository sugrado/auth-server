using AuthServer.Core.Configuration;
using AuthServer.Core.DataAccess.EntityFramework;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities.Concrete;
using AuthServer.Core.Service;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> clientOptions,
            ITokenService tokenService,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IEntityRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = clientOptions.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return new Response<TokenDto>("Email or Password is wrong", 400, true);

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new Response<TokenDto>("Email or Password is wrong", 400, true);

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenService
                .Where(x => x.UserId == user.Id)
                .SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return new Response<TokenDto>(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients
                .Where(p => p.Id == clientLoginDto.ClientId &&
                            p.Secret == clientLoginDto.ClientSecret)
                .SingleOrDefault();

            if (client == null)
                return new Response<ClientTokenDto>("ClientId or ClientSecret not found", 404, true);

            var token = _tokenService.CreateTokenByClient(client);

            return new Response<ClientTokenDto>(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService
                .Where(x => x.Code == refreshToken)
                .SingleOrDefaultAsync();

            if (existRefreshToken == null)
                return new Response<TokenDto>("Refresh token not found", 404, true);

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if (user == null)
                return new Response<TokenDto>("User Id not found", 404, true);

            var tokenDto = _tokenService.CreateToken(user);

            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();
            return new Response<TokenDto>(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService
                .Where(x => x.Code == refreshToken)
                .SingleOrDefaultAsync();

            if (existRefreshToken == null)
                return new Response<NoDataDto>("Refresh token not found", 404, true);

            _userRefreshTokenService.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();
            return new Response<NoDataDto>(200);
        }
    }
}
