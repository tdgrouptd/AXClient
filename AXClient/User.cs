using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AXClient;

namespace AXClient
{
    public class User
    {
        public string username ="";
        public string password = "";
        public User(string iusername ,string ipassword)
        {
            username = iusername;
            password = ipassword;
            try_login();
        }

        private void try_login()
        {
            try
            {
                DB db = new DB(this);
                var dt = db.exec("select 1 as a");
            }
            catch
            {
                throw new Exception("авторизация не удалась");
            }


        }
    }
}
