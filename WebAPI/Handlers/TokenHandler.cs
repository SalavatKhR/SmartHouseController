using System.IdentityModel.Tokens.Jwt;
using Microsoft.Net.Http.Headers;
using WebAPI.Models;

namespace WebAPI.Handlers;

public static class TokenHandler
{
    public static Claims GetClaims(HttpRequest request)
    {
        var token = request.Headers[HeaderNames.Authorization]
            .ToString()
            .Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var claims = new Claims (
            Id: jwt.Claims.First(claim => claim.Type == "sub").Value,
            Name: jwt.Claims.First(claim => claim.Type == "name").Value
            );
        
        return claims;
    }
}