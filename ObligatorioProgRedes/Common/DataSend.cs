using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DataSend
    {

        public static byte[] GenMenssage(string message, Header header)
        {
            byte[] codedMessage = Encoding.UTF8.GetBytes(message);
            byte[] headerBytes = header.GenRequest();

            byte[] fullMessage = new byte[headerBytes.Length + codedMessage.Length];
            Array.Copy(headerBytes,0,fullMessage,0,headerBytes.Length);
            Array.Copy(codedMessage, 0, fullMessage, headerBytes.Length, codedMessage.Length);

            return fullMessage;
        }

        public static string DecodeMessage (byte[] codedMessage)
        {
            string decodedMessage;
            int index = HeaderConstants.GetLength();
            int count = codedMessage.Length - index;

            decodedMessage = Encoding.UTF8.GetString(codedMessage,index,count);

            return decodedMessage;
        }
    }
}
