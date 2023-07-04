using System;

namespace ARIT_Hackathon.Extensions
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(TokenRequest request, out string token);
    }
}
