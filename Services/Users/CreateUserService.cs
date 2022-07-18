using WebAPI.Errors;

namespace WebAPI.Services.Users
{

    public class CreateUserService
    {
        private readonly DataContext context;

        public CreateUserService(DataContext context)
        {
            this.context = context;
        }

        public async Task<User> Execute(string login, string email, string password)
        {

            var findUserLogin = context.Users.Where(user => user.Login == login).FirstOrDefault();

            if (findUserLogin != null) throw new AppException("User already exists!");


            var findUserEmail = context.Users.Where(user => user.Email == email).FirstOrDefault();

            if (findUserEmail != null) throw new AppException("User already exists!");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            User user = new()
            {
                Login = login,
                Email = email,
                Password = passwordHash
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            return user;

        }
    }
}
