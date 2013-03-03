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

        public void Save(User user)
        {
            lock (locker)
            {
                using (var session = this.database.OpenSession())
                {
                    var users = session.Load<Users>("users");

                    if (users == null) users = new Users();

                    // might already exist
                    if (users.Items.Where(u => u.UserName.ToLower() == user.UserName.ToLower()).Any())
                    {
                        return; // TODO: do something better
                    }

                    users.Items.Add(user);
                    session.Store(users);
                    session.SaveChanges();
                }
            }
        }
    }
}
