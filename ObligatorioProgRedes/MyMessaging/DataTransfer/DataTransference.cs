using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyMessaging.DataTransference
{
    public class DataTransference
    {
        public static void SendData(byte[] message, Socket socket)
        {
            socket.Send(message, 0, message.Length, SocketFlags.None);
        }

        public static DataTransferResult RecieveData(Socket socket)
        {
            DataTransferResult result = new DataTransferResult();
            int dataLength;

            int headerLength = HeaderConstants.GetLength();
            var headerBytes = new byte[headerLength];
            int received = 0;
            bool exit = false;
            while (received < headerLength && !exit)
            {
                try
                {
                    received += socket.Receive(headerBytes, received, headerLength - received, SocketFlags.None);
                    if (received == 0)
                        exit = true;
                }catch (System.Net.Sockets.SocketException)
                {
                    exit = true;
                    Console.WriteLine("La conexion se cerro de forma abrupta");
                }
            }
            if (!exit)
            {
                Header header = new Header(headerBytes);
                result.Header = header;
                dataLength = header.GetDataLength();

                var data = new byte[dataLength];
                received = 0;
                while (received < dataLength)
                {
                    received += socket.Receive(data, received, dataLength - received, SocketFlags.None);
                }

                //var word = DecodeMessage(data);
                result.objectResult = data;
                return result;
            }
            else
                return null;
        }
    }
}
