using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging.DataTransfer
{
    public class ByteDataTransfer : DataTransferSuper
    {
        protected override byte[] CastMessage(object obj)
        {
            return (byte[])obj;
        }

        protected override object DecodeMessage(byte[] data)
        {
            return data;
        }
    }
}
