using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemRezerwacjiWizyt.Utils
{
    public static class Token
    {
        public static string GetToken()
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                bool numSign = r.Next(2) == 1;
                sb.Append(numSign ? (char)r.Next(49, 58) : (char)r.Next(65, 91));
            }
            return sb.ToString();
        }
    }
}
