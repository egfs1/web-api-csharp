using WebAPI.Errors;

namespace WebAPI.Services.Users
{
    public class DeleteUserService
    {
        private readonly DataContext context;

        public DeleteUserService(DataContext context)
        {
            this.context = context;
        }

        public async Task Execute(string id)
        {
            var user = await context.Users.FindAsync(Guid.Parse(id));

            if (user == null) throw new AppException("User not found!");

            context.Users.Remove(user);

            await context.SaveChangesAsync();

        }
    }
}
