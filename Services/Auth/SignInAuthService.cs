using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Errors;
using WebAPI.Helpers;

namespace WebAPI.Services.Auth
{

    public class SignInAuthService
    {
        private readonly DataContext context;
        private readonly AppSettings appSettings;

        public SignInAuthService(DataContext context, AppSettings appSettings)
        {
            this.context = context;
            this.appSettings = appSettings;
        }

        public string Execute(string login, string password)
        {
            var user = context.Users.Where(user => user.Login == login).FirstOrDefault();

            if (user == null) throw new AppException("User/Password is wrong!");

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!passwordMatch) throw new AppException("User/Password is wrong!");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
