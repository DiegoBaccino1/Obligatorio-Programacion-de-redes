﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging
{
    public class BoolDataTransfer : DataTransferSuper
    {
        protected override byte[] CastMessage(object obj)
        {
            bool data = (bool)obj;
            string dataTransfer = data.ToString();
            return Encoding.UTF8.GetBytes(dataTransfer);
        }

        protected override object DecodeMessage(byte[] data)
        {
            string rawData = Encoding.UTF8.GetString(data);
            return Boolean.Parse(rawData);
        }
    }
}
