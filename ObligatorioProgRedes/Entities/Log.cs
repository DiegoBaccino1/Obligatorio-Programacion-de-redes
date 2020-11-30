using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Log
    {
        public const string WARNING_LEVEL= "Warning";
        public const string SUCCESS_LEVEL = "Success";
        public Log()
        {
        }

        public string Message { get; set; }
        public string Username { get; set; }
        public int Command { get; set; }
        public string Level { get; set; } 
        public DateTime Date { get; set; }
    }
}
