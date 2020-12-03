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
        public List<string> Comments { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {

            try
            {
                if (obj == null || obj.GetType() != this.GetType()) throw new Exception();
                Photo photo = (Photo)obj;
                return photo.Name.Equals(this.Name);
            }
            catch(Exception)
            {
                return false;
            }
            
            
        }
    }
}
