using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ARIT_Hackathon.Extensions
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenManagement _tokenManagement;

        private List<UserRequest> _useList = new List<UserRequest>
        {
            new UserRequest { LoginId = "user1", Password = "user@2023"}
        };
        public TokenAuthenticationService(IOptions<TokenManagement> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }
        public bool IsAuthenticated(TokenRequest request, out string token)
        {
            token = string.Empty;
            //if (!_userManagementService.IsValidUser(request.Username, request.Password)) return false;
            UserRequest user = _useList.Find(x => x.LoginId == request.LoginId && x.Password == request.Password);
            if (user == null)
            {
                return false;
            }
            var claim = new[]
            {
                new Claim(ClaimTypes.Name, request.LoginId)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _tokenManagement.Issuer,
                _tokenManagement.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;
        }
    }
}
