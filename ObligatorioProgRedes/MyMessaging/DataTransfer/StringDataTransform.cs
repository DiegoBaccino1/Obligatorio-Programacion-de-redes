using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging
{
    public class StringDataTransform:DataTransformSuper
    {
        protected override byte[] CastMessage(object obj)
        {
            string data = (string)obj;
            return Encoding.UTF8.GetBytes(data);
        }

        public override object DecodeMessage(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
