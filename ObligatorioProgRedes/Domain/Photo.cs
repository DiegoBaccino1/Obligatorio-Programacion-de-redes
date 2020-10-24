using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Photo
    {
        public byte[] File { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public List<string> Comments { get; set; }
    }
}
