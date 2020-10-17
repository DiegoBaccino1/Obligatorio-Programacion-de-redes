using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private static Socket _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static IPAddress address = IPAddress.Parse("127.0.0.1");

        private static int port = 6000;

        private const int BUFFER_SIZE = 1024;

        private const int CONNECTIONS = 10;

        private static bool isServerUp = false;


        public void StartServer()

        {

            IPEndPoint endPoint = new IPEndPoint(address, port);

            isServerUp = true;

            _server.Bind(endPoint);

            _server.Listen(CONNECTIONS);

            Console.WriteLine("Listening.....");

            while (isServerUp)

            {

                Socket _clientSocket = _server.Accept();

                new Thread(() => HandleClient(_clientSocket)).Start();

            }

        }

        public void HandleClient(Socket socket)
        {
           
            while (true)
            {
                int command;
                int dataLength;
                string direction;
                int headerLength = HeaderConstants.GetLength();
                var header = new byte[headerLength];
                int received = 0;
                
                while (received < headerLength)
                {
                    received += socket.Receive(header, received, headerLength - received, SocketFlags.None);
                }
                
                Header header2 = new Header(header);
                direction=header2.GetDirection();
                command=header2.GetCommand();
                dataLength=header2.GetDataLength();
                
                var data = new byte[dataLength];
                received = 0;
                while (received < dataLength)
                {
                    received += socket.Receive(data, received, dataLength - received, SocketFlags.None);
                }
                
                var word = Encoding.UTF8.GetString(data);
                
                Console.WriteLine("Client says: " + word);
            }

        }

    }
}
