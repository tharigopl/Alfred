using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Embedded;

namespace Bot.Data
{
    public class Database
    {
        private static IDocumentStore store;
        private static object locker = new object();

        private IDocumentSession session;

        public Database()
        {
            Database.Initialize();
        }

        public static void Initialize()
        {
            if (Database.store == null)
            {
                lock (locker)
                {
                    if (Database.store == null)
                    {
                        Database.store = new EmbeddableDocumentStore {
                            ConnectionStringName = "Local",
                            UseEmbeddedHttpServer = true
                        };
                        Database.store.Initialize();
                    }
                }
            }
        }

        public void SaveChanges()
        {
            if (this.session != null)
            {
                this.session.SaveChanges();
            }
        }

        public IDocumentSession OpenSession()
        {
            return Database.store.OpenSession();
        }

    }

}
