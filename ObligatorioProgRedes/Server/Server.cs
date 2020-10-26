using Common;
using Domain;
using MyMessaging.Responses;
using Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MyMessaging;

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

        private static List<User> Users = new List<User>();
        

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
            DataTransferSuper transfer = new StringDataTransfer();
            Response response = new StringResponse();
            User user;
            while (true)
            {
                
                DataTransferResult result = transfer.RecieveData(socket);

                int command=result.Header.GetCommand();
                int dataLength= result.Header.GetDataLength();
                string direction= result.Header.GetDirection();
                var word = (string)result.objectResult;
                switch (command)
                {
                    case 2:
                        try
                        {
                            user = Login(word);
                            response = new StringResponse();
                            var ret = "true";
                            response.SendResponse(command, ret, socket, ret.Length);
                            break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    case 1:
                        try
                        {
                            SignUp(word);
                            break;
                        }
                        catch (UserAlreadyExistException)
                        {
                            break;
                        }
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
                //Response(command,messageResponse,socket);
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
            lock (Users)
            {
                if (Users.Contains(user))
                {
                    if (!user.IsLogged)
                    {
                        user.IsLogged = true;
                        return user;
                    }
                    else
                        throw new IsLoggedException();
                }
                else
                    throw new UserNotExistException();
            }
        }

        private static User GetUserByCredentials(string username, string password)
        {
            lock (Users)
            {
                return Users.Where
                    (x => x.Username.Equals(username) && x.Password.Equals(password)).FirstOrDefault();
            }
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
