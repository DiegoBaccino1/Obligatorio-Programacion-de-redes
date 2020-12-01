using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging.DataTransfer
{
    public class ByteDataTransform : DataTransformSuper
    {
        protected override byte[] CastMessage(object obj)
        {
            return (byte[])obj;
        }

        public override object DecodeMessage(byte[] data)
        {
            return data;
        }
    }
}
