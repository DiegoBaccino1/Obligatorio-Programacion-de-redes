using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging.DataTransfer
{
    public class ListStringDataTransfer : DataTransferSuper
    {
        private const char SEPARATOR_TRANSFER = '-';

        protected override byte[] CastMessage(object obj)
        {
            var listData = obj as List<string>;
            string dataSting="";
            foreach (string s in listData) 
            {
                dataSting += s + SEPARATOR_TRANSFER.ToString();
            }
            return Encoding.UTF8.GetBytes(dataSting);
        }
        public static int ListLength(List<string> list)
        {
            var ret = "";
            foreach (string s in list)
            {
                ret += s + SEPARATOR_TRANSFER.ToString();
            }
            return ret.Length;
        }
        protected override object DecodeMessage(byte[] data)
        {
            List<string> list = new List<string>();
            string stringData = Encoding.UTF8.GetString(data);
            string[] rawData = stringData.Split(SEPARATOR_TRANSFER);
            for (int i = 0; i < rawData.Length; i++)
            {
                if (!string.IsNullOrEmpty(rawData[i]))
                    list.Add(rawData[i]);
            }
            return list;
        }
    }
}
