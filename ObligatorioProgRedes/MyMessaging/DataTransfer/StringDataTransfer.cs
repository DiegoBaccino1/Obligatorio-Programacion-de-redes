using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging
{
    public class StringDataTransfer:DataTransferSuper
    {
        protected override byte[] CastMessage(object obj)
        {
            string data = (string)obj;
            return Encoding.UTF8.GetBytes(data);
        }

        protected override object DecodeMessage(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
