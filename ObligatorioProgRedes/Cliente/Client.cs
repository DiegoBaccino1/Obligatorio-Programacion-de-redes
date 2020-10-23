using System;
using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cliente
{
    public class Client
    {
        private const string SEPARATOR = "%";
        private static bool IsConectedToServer;


        private static readonly IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

        private static readonly IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

        private static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

       
        public void ConectToServer()
        {
            try
            {
                socket.Bind(clientEndPoint);

                socket.Connect(serverEndPoint);

                Console.WriteLine("Conectado al servidor\n");
                Console.WriteLine("Bienvenido.....\n");

                Console.WriteLine("Ingrese usuario:\n");
                var userName = Console.ReadLine();
                Console.WriteLine("Ingrese Contrasenia\n");
                var userPassword = Console.ReadLine();

                var IsLogged = false;

                var credentials = userName + SEPARATOR + userPassword;
                Header header = new Header(HeaderConstants.Request, CommandConstants.SignUp, credentials.Length);
                var codedMessage = DataSend.GenMenssage(credentials, header);
                SendData(codedMessage);

                //IsLogged = RecivedData();
                //List<string> ListaUsuario=.Value as List<string>
                while (true) { }
                if (IsLogged)
                {
                    DisplayMenu();
                }
                while (IsLogged)
                {
                    int option = -1;
                    int command = -1;
                    string data = "";
                    byte[] codedRequest = new byte[1];                    
                    option = GetOption(option);
                    switch (option)
                    {
                        case 2:
                            //UpLoadPhoto();
                            break;
                        case 3:
                            command=CommandConstants.ListUsers;
                            break;
                        case 4:
                            command=CommandConstants.ListFiles;
                            break;
                        case 5:
                            command=CommandConstants.ViewComents;
                            break;
                        case 6:
                            command=CommandConstants.AddComent;
                            data = Console.ReadLine();                            
                            break;
                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                    header = new Header(HeaderConstants.Request, command, data.Length);
                    codedRequest = DataSend.GenMenssage(data, header);
                    SendData(codedRequest);
                }
            }
            catch (Exception)
            {
            }
        }

        private static int GetOption(int option)
        {
            try
            {
                option = Int32.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Option");
            }
            return option;
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("1- Cargar foto\n");
            Console.WriteLine("2- Listado de usuarios\n");
            Console.WriteLine("3- Listado de fotos de un usuario\n");
            Console.WriteLine("4- Ver comentarios de una foto\n");
            Console.WriteLine("5- Agregar comentarios a una foto\n");
        }

        private void SendData(byte[] message)
        {
            socket.Send(message, 0,message.Length, SocketFlags.None);
        }
    }
}
