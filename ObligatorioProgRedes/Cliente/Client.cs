using System;
using Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MyMessaging;
using MyMessaging.DataTransfer;
using System.Runtime.InteropServices;

namespace Cliente
{
    public class Client
    {
        private const string SEPARATOR = "%";//psar a common


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
                int option = -1;
                bool exit = false;
                string credentials ="";
                string username = "";
                string userPassword = "";
                Header header;
                DataTransferSuper dataTransferReciver=new StringDataTransfer();
                DataTransferSuper dataTransferSender = new StringDataTransfer();
                while (!exit)
                {
                    DisplayStarMenu();

                    option = GetOption(option);
                    Console.WriteLine("Ingrese usuario:\n");
                    username = Console.ReadLine();
                    Console.WriteLine("Ingrese Contrasenia\n");
                    userPassword = Console.ReadLine();
                    credentials = username + SEPARATOR + userPassword;

                    switch (option)
                    {
                        case 1:
                            option = CommandConstants.SignUp;
                            break;
                        case 2:
                            option = CommandConstants.Login;
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Opcion Invalida");
                            break;
                    }
                    header = new Header(HeaderConstants.Request, option, credentials.Length);
                    var codedMessage = dataTransferSender.GenMenssage(credentials, header);
                    DataTransferSuper.SendData(codedMessage, socket);
                }
                dataTransferReciver = new StringDataTransfer();
                DataTransferResult result = dataTransferReciver.RecieveData(socket);

                dynamic resultData = (string)result.objectResult;
                bool isLogged =  Boolean.Parse(resultData);
                if (isLogged)
                {
                    DisplayMenu();
                }
                while (isLogged)
                {
                    option = -1;
                    int command = -1;
                    string data = "";
                    byte[] codedRequest = new byte[1];
                    option = GetOption(option);
                    switch (option)
                    {
                        case 3:
                            //UpLoadPhoto();
                            break;
                        case CommandConstants.ListUsers:
                            command = CommandConstants.ListUsers;
                            dataTransferReciver = new ListStringDataTransfer();
                            //result = dataTransfer.RecieveData(socket);
                            //resultData = result.objectResult as List<string>;
                            //Display(resultData);
                            break;
                        case 5:
                            command = CommandConstants.ListFiles;
                            break;
                        case 6:
                            command = CommandConstants.ViewComents;
                            break;
                        case 7:
                            command = CommandConstants.AddComent;
                            data = Console.ReadLine();
                            break;
                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                    header = new Header(HeaderConstants.Request, command, data.Length);
                    codedRequest = dataTransferSender.GenMenssage(data, header);
                    DataTransferSuper.SendData(codedRequest, socket);

                    result = dataTransferReciver.RecieveData(socket);
                    resultData = result.objectResult as List<string>;
                    Display(resultData);

                }
            }
            catch (Exception)
            {
            }
        }

        private static void DisplayStarMenu()
        {
            Console.WriteLine(CommandConstants.SignUp + "-Alta Usuario");
            Console.WriteLine(CommandConstants.Login + "-Log in");
        }

        private static void Display(List<string> list)
        {
            int i = 0;
            Console.WriteLine("Usuarios conectados:\n");
            foreach (string username in list)
            {
                Console.WriteLine(++i + " - " + username);
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
            Console.WriteLine("\n"+CommandConstants.UploadFile+"- Cargar foto\n");
            Console.WriteLine(CommandConstants.ListUsers+"- Listado de usuarios\n");
            Console.WriteLine(CommandConstants.ListFiles+"- Listado de fotos de un usuario\n");
            Console.WriteLine(CommandConstants.ViewComents+"- Ver comentarios de una foto\n");
            Console.WriteLine(CommandConstants.AddComent+"- Agregar comentarios a una foto\n");
        }
    }
}
