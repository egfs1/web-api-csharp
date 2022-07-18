using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPI.Helpers;

namespace WebAPI.Services.Auth
{

    public class MeAuthService
    {
        private readonly AppSettings appSettings;
        private readonly DataContext context;

        public MeAuthService(DataContext context, AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.context = context;
        }

        public User? Execute(string token)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                var user = context.Users.Find(Guid.Parse(userId));

                return user;
            }
            catch
            {
                return null;
            }

        }
    }
}
