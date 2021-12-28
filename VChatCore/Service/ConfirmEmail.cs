using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VChatCore.Model;

namespace VChatCore.Service
{
    public class ConfirmEmail
    {

        protected readonly MyContext context;

        public ConfirmEmail(MyContext context)
        {
            this.context = context;
        }

        public string ConfirmEmailInfo(string username)
        {
            var user = GetUser(username);
            if (user ==null)
            {
                return "Not Success";
            }
            user.IsConfirmEmail = true;
            this.context.SaveChanges();
            return "CONFIRM YOUR ACCOUNT SUCCESS!";
        }
        public User GetUser(string username)
        {
            var user = this.context.Users.Where(x => x.UserName.Equals(username)).SingleOrDefault();
            return user;
        }
    }
}
