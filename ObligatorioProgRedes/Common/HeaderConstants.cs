using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class HeaderConstants
    {

        public static string Request = "REQ";

        public static string Response = "RES";

        public static int CommandLength = 2;

        public static int DataLength = 4;



        public static int GetLength()
        {
            return Request.Length + CommandLength + DataLength;
        }

    }
}
