using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinSelfHostWebAPI.Providers
{
    public static class UserConfig
    {
        private static string userName = "test";
        private static string userPassword = "test";

        public static bool IsUserAllowd(string name, string password)
        {
            return name == userName && password == userPassword;
        }
    }
}
