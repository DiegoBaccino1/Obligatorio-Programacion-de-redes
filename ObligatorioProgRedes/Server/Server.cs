using Common;
using Domain;
using Exceptions;
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
        private const string OK_MESSAGE_RESPONSE = "OK";
        private static bool isServerUp = false;

        private static List<User> Users = new List<User>();

        private static List<User> UsersLogged = new List<User>();
        

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
            User user;
            while (true)
            {
                DataTransferResult result = DataTransfer.RecieveData(socket);
                
                int command=result.Header.GetCommand();
                int dataLength= result.Header.GetDataLength();
                string direction= result.Header.GetDirection();
                var word = (string)result.objectResult;
                var messageResponse = "";
                switch (command)
                {
                    case 2:
                        try
                        {
                            user = Login(word);
                            messageResponse = OK_MESSAGE_RESPONSE;
                            break;
                        }
                        catch (Exception)
                        {
                            messageResponse = "Error";
                            break;
                        }
                    case 1:
                        try
                        {
                            SignUp(word);
                            messageResponse = OK_MESSAGE_RESPONSE;
                            break;
                        }
                        catch (UserAlreadyExistException)
                        {
                            messageResponse = "Usuraio ya existe";
                            break;
                        }
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
                Response(command,messageResponse,socket);
            }

        }

        private static void GetCredentials(string word, out string username, out string password)
        {
            var credentials = word.Split('%');
            username = credentials[0];
            password = credentials[1];
        }

        private static User Login(string credentials)
        {
            string username, password;
            GetCredentials(credentials, out username, out password);
            User user = GetUserByCredentials(username, password);
            bool exist = false;
            lock (Users)
            {
                exist = Users.Contains(user);
            }
            //No me pueden borrar el user de la lista?
            if (exist)
            {
                lock (UsersLogged)
                {
                    if (!UsersLogged.Contains(user))
                    {
                        UsersLogged.Add(user);
                        return user;
                    }
                    else
                        throw new IsLoggedException();
                }
            }
            else
                throw new UserNotExistException();
        }

        private static User GetUserByCredentials(string username, string password)
        {
            lock (Users)
            {
                return Users.Where
                    (x => x.Username.Equals(username) && x.Password.Equals(password)).FirstOrDefault();
            }
        }

        private static bool IsLogged(User user)
        {
            return UsersLogged.Contains(user);
        }
        
        private static User CreateUser(string username, string password)
        {
            User user = new User();
            user.Username = username;
            user.Password = password;
            user.Photos = new List<Photo>();
            return user;
        }

        private static void SignUp(string credentials)
        {
            string username, password;
            GetCredentials(credentials, out username, out password);

            User user = CreateUser(username, password);
            lock (Users)
            {
                if (!Users.Contains(user))
                {
                    Users.Add(user);
                }
                else
                    throw new UserAlreadyExistException();
            }
        }

        private static void Response(int command,string message,Socket socket)
        {
            Header header = new Header(HeaderConstants.Response, command, message.Length);
            var byteMessage=DataTransfer.GenMenssage(message, header);
            DataTransfer.SendData(byteMessage,socket);
        }

        private static bool BoolResponse(int command)
        {
            return command == CommandConstants.Login || command == CommandConstants.AddComent || command == CommandConstants.SignUp || command == CommandConstants.UploadFile;
        }
    }
}
