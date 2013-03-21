using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Data
{
    public class UsersRepository
    {
        private readonly Database database;

        private static object locker = new object();

        public UsersRepository()
        {
            this.database = new Database();
        }

        public bool Save(User newUser)
        {
            lock (locker)
            {
                using (var session = this.database.OpenSession())
                {
                    var users = session.Load<Users>("users") ?? new Users();

                    if (UserAlreadyExists(newUser, users))
                    {
                        return false;
                    }

                    users.Items.Add(newUser);
                    session.SaveChanges();
                }
            }

            return true;
        }

        private bool UserAlreadyExists(User newUser, Users users)
        {
            return users
                .Items
                .Any(
                    user =>
                    user.UserName.Equals(
                        newUser.UserName,
                        StringComparison.OrdinalIgnoreCase
                    )
                );
        }
    }
}
