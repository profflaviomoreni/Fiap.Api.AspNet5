using Fiap.Api.AspNet5.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fiap.Api.AspNet5.Services
{
    public class AuthenticationService
    {

        public static string GetToken(UsuarioModel model)
        {
            byte[] secret = Encoding.ASCII.GetBytes(Settings.Secret);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity( new Claim[]
                {
                    new Claim( ClaimTypes.Name, model.NomeUsuario ),
                    new Claim( ClaimTypes.Role, model.Regra)
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials( 
                    new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature )
            };

            SecurityToken token = handler.CreateToken( securityTokenDescriptor );

            return handler.WriteToken(token);
        }



    }
}
