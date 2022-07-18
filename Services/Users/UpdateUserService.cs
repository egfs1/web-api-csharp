using WebAPI.Errors;

namespace WebAPI.Services.Users
{
    public class UpdateUserService
    {
        private readonly DataContext context;

        public UpdateUserService(DataContext context)
        {
            this.context = context;
        }

        public async Task<User> Execute(string id, string login, string email, string password, string oldPassword)
        {
            // find user
            var user = await context.Users.FindAsync(Guid.Parse(id));

            if (user == null) throw new AppException("User not found!");

            // verify if new login or email is not used
            var findUserLogin = context.Users.Where(user => user.Login == login).FirstOrDefault();

            if (findUserLogin != null && findUserLogin.Id != user.Id) throw new AppException("User already exists!");

            var findUserEmail = context.Users.Where(user => user.Email == email).FirstOrDefault();

            if (findUserEmail != null && findUserEmail.Id != user.Id) throw new AppException("User already exists!");

            // verify if oldPassword match user password and it's not empty
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(oldPassword, user.Password);

            if (!passwordMatch && oldPassword != string.Empty) throw new AppException("Old password is wrong!");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // update user data
            if (login != string.Empty) user.Login = login;

            if (email != string.Empty) user.Email = email;

            if (password != string.Empty) user.Password = passwordHash;

            await context.SaveChangesAsync();

            return user;

        }
    }
}
