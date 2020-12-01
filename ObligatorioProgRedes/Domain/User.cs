using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Photo> Photos { get; set; }
        public bool IsLogged { get; set; }

        public void AddPhoto(Photo photo)
        {
            if(this.Photos == null)
            {
                this.Photos = new List<Photo>();
                this.Photos.Add(photo);
            }
            else
            {
                this.Photos.Add(photo);
            }
        }
        public override bool Equals(object obj)
        {
            try
            {
                User user = (User)obj;
                return user.Username.Equals(this.Username);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
