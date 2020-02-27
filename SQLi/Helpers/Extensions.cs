using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLi.Helpers
{
    public static class Extensions
    {
        public static string Join(this string[] strs)
        {
            string resp = "";
            foreach (string s in strs)
            {
                resp += s;
            }
            return resp;
        }
    }
}
