using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Errors;
using WebAPI.Helpers;
using WebAPI.Services.Auth;
using WebAPI.Services.Users;

namespace WebAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext context;
        private readonly AppSettings appSettings;

        public AuthController(DataContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult<List<object>>> SignUp([FromBody] SignUpRequest request)
        {

            Console.WriteLine(request.Login);
            Console.WriteLine(request.Email);
            Console.WriteLine(request.Password);

            CreateUserService createUserService = new(context);

            SignInAuthService signInAuthService = new(context, appSettings);

            User user = await createUserService.Execute(request.Login, request.Email, request.Password);

            string token = signInAuthService.Execute(request.Login, request.Password);

            return Ok(new { token = token, user  = user});

        }

        [HttpPost]
        [Route("signin")]
        public ActionResult<List<object>> SignIn([FromBody] SignInRequest request)
        {
            SignInAuthService signInAuthService = new(context, appSettings);

            MeAuthService meAuthService = new(context, appSettings);

            string token = signInAuthService.Execute(request.Login, request.Password);

            var user = meAuthService.Execute(token);

            return Ok(new { token = token, user  = user});

        }

        [Helpers.Authorize]
        [HttpPost]
        [Route("signout/{id}")]
        public async Task<ActionResult<bool>> SignOut(string id)
        {
            if (!id.Equals(HttpContext.Items["UserId"])) throw new AppException("Unauthorazed");

            SignOutAuthService signOutAuthService = new();

            bool signedOut = await signOutAuthService.Execute(id);

            return Ok(new {signedOut = signedOut});

        }

        [HttpPost]
        [Route("me")]
        public ActionResult<User> Me([FromBody] MeRequest request)
        {

            MeAuthService meAuthService = new(context, appSettings);

            var user = meAuthService.Execute(request.Token);

            return Ok(user);
        }

    }

    public class SignUpRequest
    {

        public string Login { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class SignInRequest
    {
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

    }

    public class MeRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}

