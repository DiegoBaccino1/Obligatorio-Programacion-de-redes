﻿using Common;
using Common.Interfaces;
using Domain;
using Entities;
using Exceptions;
using LogServerImp;
using MyMessaging;
using MyMessaging.DataTransfer;
using MyMessaging.DataTransference;
using MyMessaging.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Server
{
    public class Server
    {
        public static Socket _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static IPAddress address = IPAddress.Parse(ConectionConstants.HOST);

        public static IPEndPoint endPoint = new IPEndPoint(address, ConectionConstants.SERVERPORT);

        public static bool isServerUp = false;

        public static List<Socket> _conectedClients = new List<Socket>();

        public static List<User> Users = new List<User>();

        public void StartServer()
        {
            _server.Bind(endPoint);

            _server.Listen(ConectionConstants.CONNECTIONS);

            Console.WriteLine("Listening.....");
            while (isServerUp)
            {
                Socket _clientSocket = _server.Accept();

                _conectedClients.Add(_clientSocket);

                new Thread(() => HandleClient(_clientSocket)).Start();
                Console.WriteLine(CommandConstants.Disconnect + " Desconectar");
                int serverOption = GetOption();
                if (serverOption == CommandConstants.Disconnect)
                    EndConnection();
            }
            Console.WriteLine("Connection ended");
        }

        private void EndConnection()
        {
            isServerUp = false;
            _server.Close(timeout: 0);
            foreach (Socket client in _conectedClients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            var trapSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                trapSocket.Connect(endPoint);
            }
            catch (SocketException)
            {
                Console.WriteLine("Conexion cerrada");
            }
        }
        private int GetOption()
        {
            int retorno = 0;
            bool exit = false;
            while (!exit)
            {
                try
                {
                    Console.WriteLine("Ingrese la opcion\n");
                    retorno = Int32.Parse(Console.ReadLine());
                    exit = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Formato invalido para la opcion");
                }
            }
            return retorno;
        }

        public void HandleClient(Socket socket)
        {
            DataTransformSuper transfer = new StringDataTransform();
            Response response = new StringResponse();
            User user = new User();
            bool endConn = false;
            long fileSize = 0;
            string fileName = "";
            while (isServerUp && !endConn)
            {
                DataTransferResult result = DataTransference.RecieveData(socket);
                if (result != null)
                {
                    int command = result.Header.GetCommand();
                    int dataLength = result.Header.GetDataLength();
                    string direction = result.Header.GetDirection();
                    string word = "";
                    Log log = new Log()
                    {
                        Command = command,
                        Date = DateTime.Now,
                        
                    };
                    if (command != 31)
                    {
                        transfer = new StringDataTransform();
                        result.objectResult = transfer.DecodeMessage((byte[])result.objectResult);
                        word = (string)result.objectResult;
                        log.Message = word;
                    };
                    dynamic responseData;
                    

                    switch (command)
                    {
                        case CommandConstants.Login:
                            try
                            {
                                user = Login(word);
                                response = new StringResponse();
                                responseData = "true";
                                response.SendResponse(command, responseData, socket, responseData.Length);
                                SendSuccessfullLog(user, log);
                                break;
                            }
                            catch (Exception)
                            {
                                response = new StringResponse();
                                responseData = "false";
                                response.SendResponse(command, responseData, socket, responseData.Length);
                                SendWarningLog(log);
                                break;
                            }
                        case CommandConstants.SignUp:
                            try
                            {
                                SignUp(word);
                                SendSuccessfullLog(user, log);
                                break;
                            }
                            catch (UserAlreadyExistException)
                            {
                                SendWarningLog(log);
                                break;
                            }
                        case CommandConstants.ListUsers:
                            try
                            {
                                List<string> usersList = GetUsers();
                                response = new ListStringResponse();
                                responseData = usersList;
                                int responseDataLength = ListStringDataTransform.ListLength(usersList);
                                response.SendResponse(command, responseData, socket, responseDataLength);
                                SendSuccessfullLog(user, log);
                                break;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Ocurrio un error al listar los usuarios");
                                SendWarningLog(log);
                                break;
                            }

                        case CommandConstants.ListFiles:
                            try
                            {
                                User userPhoto = new User();
                                userPhoto.Username = word;
                                List<string> fileList = GetUserPhotos(userPhoto);
                                response = new ListStringResponse();
                                responseData = fileList;
                                int responseDataLength = ListStringDataTransform.ListLength(fileList);
                                if (responseDataLength == 0) throw new Exception();
                                response.SendResponse(command, responseData, socket, responseDataLength);
                                SendSuccessfullLog(user, log);
                                break;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Ocurrio un problema al listar los archivos");
                                response = new StringResponse();
                                responseData = "Error al listar fotos";
                                response.SendResponse(Common.CommandConstants.ErrorListing, responseData, socket, responseData.Length);
                                SendWarningLog(log);
                                break;
                            }
                        case CommandConstants.UploadFile:
                            ReciveFileData(word, out fileName, out fileSize);
                            transfer = new ByteDataTransform();
                            Photo photo1 = new Photo();
                            photo1.Name = fileName;
                            photo1.Comments = new List<string>();
                            user.AddPhoto(photo1);

                            SendSuccessfullLog(user, log);
                            break;

                        case CommandConstants.UploadFileSignal:
                            byte[] fileBytes = (byte[])result.objectResult;
                            IFileSenderHandler senderHandler = new FileSenderHandler();
                            senderHandler.Write(fileName, fileBytes);
                            log.Username = user.Username;
                            log.Message = "File part";
                            SendLog(log);
                            break;
                        case CommandConstants.AddComent:
                            try
                            {
                                AddComment(word);
                                SendSuccessfullLog(user, log);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Ocurrio un problema al agregar el comentario");
                                SendWarningLog(log);
                            }
                            break;
                        case CommandConstants.ViewComents:
                            try
                            {
                                List<string> comments;
                                string userName, photo;
                                GetCredentials(word, out userName, out photo);
                                Photo photoForComments = GetPhoto(userName, photo);
                                comments = photoForComments.Comments;

                                response = new ListStringResponse();
                                responseData = comments;
                                int responseDataLength = ListStringDataTransform.ListLength(comments);
                                response.SendResponse(command, responseData, socket, responseDataLength);
                                SendSuccessfullLog(user, log);
                                break;
                            }catch (Exception)
                            {
                                Console.WriteLine("Error al mostrar comentarios de la foto");
                                response = new StringResponse();
                                responseData = "Error al listar fotos";
                                response.SendResponse(Common.CommandConstants.ErrorListing, responseData, socket, responseData.Length);
                                SendWarningLog(log);
                                break;
                            }

                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                }
                else
                {
                    socket.Close();
                    _conectedClients.Remove(socket);
                    endConn = true;
                }
            }
        }

        private static void SendWarningLog(Log log)
        {
            log.Level = Log.WARNING_LEVEL;
            log.Username = "N/A";
            SendLog(log);
        }

        private static void SendSuccessfullLog(User user, Log log)
        {
            log.Level = Log.SUCCESS_LEVEL;
            log.Username = user.Username;
            SendLog(log);
        }
        private static void SendLog(Log log)
        {
            ILogServer logServer = new LogServerRabbitMQ();
            logServer.PublishLog(log);
        }
        private void AddComment(string word)
        {
            try
            {
                lock (Users)
                {
                    string userName, photo, comment;
                    GetDataComment(word, out userName, out photo, out comment);
                    Photo photoToComment = GetPhoto(userName, photo);
                    photoToComment.Comments.Add(comment);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Ocurrio un error al agregar el comentario");
            }
        }

        private void GetDataComment(string word, out string userName, out string photo, out string comment)
        {
            var data = word.Split('%');
            userName = data[0];
            photo = data[1];
            comment = data[2];
        }


        private void ReciveFileData(string data, out string fileName, out long fileSize)
        {
            string fileSizeAux;
            GetCredentials(data, out fileName, out fileSizeAux);
            fileSize = long.Parse(fileSizeAux);
        }

        private void ReciveFile(long fileSize, string fileName, Socket socket)
        {
            var segments = (fileSize / FileSenderHandler.FileSegmentSize);
            segments = segments * FileSenderHandler.FileSegmentSize == fileSize ? segments : segments + 1;

            long offset = 0;
            long currentSegments = 1;

            IFileSenderHandler senderHandler = new FileSenderHandler();
            while (fileSize > offset)
            {
                ByteDataTransform transfer = new ByteDataTransform();
                DataTransferResult result = DataTransference.RecieveData(socket);
                if (result.Header.GetCommand() == 31)
                {
                    byte[] fileDataRecived = (byte[])result.objectResult;
                    int size = 0;
                    if (currentSegments == segments)
                    {
                        size = (int)(fileSize - offset);
                    }
                    else
                    {
                        size = (int)FileSenderHandler.FileSegmentSize;
                    }

                    senderHandler.Write(fileName, fileDataRecived);
                    offset += size;
                    currentSegments++;
                }
            }
        }

        private List<string> GetUserPhotos(User userPhoto)
        {
            List<Photo> fileList = new List<Photo>();
            lock (Users)
            {
                foreach (User user in Users)
                {
                    if (userPhoto.Equals(user))
                    {
                        fileList = user.Photos;
                    }
                }
            }
            List<string> result = new List<string>();
            if (fileList.Count == 0)
            {
                return null;
            }
            foreach (Photo photo in fileList)
            {
                result.Add(photo.ToString());
            }
            return result;
        }

        private List<string> GetUsers()
        {
            List<string> users = new List<string>();
            lock (Users)
            {
                foreach (User user in Users)
                {
                    users.Add(user.Username);
                }
            }
            return users;
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

        private static User GetUserByName(string userName)
        {
            lock (Users)
            {
                return Users.Where
                    (x => x.Username.Equals(userName)).FirstOrDefault();
            }
        }

        private static Photo GetPhoto(string userName, string photo)
        {
            try
            {
                lock (Users)
                {
                    User user = GetUserByName(userName);
                    List<Photo> photos = user.Photos;
                    foreach (Photo p in photos)
                    {
                        if (p.ToString().Equals(photo))
                        {
                            return p;
                        }
                    }
                    throw new Exception();
                }
            }
            catch(Exception)
            {
                Console.WriteLine("No existe la foto");
                return null;
            }

        }

        public static User CreateUser(string username, string password)
        {
            User user = new User();
            user.Username = username;
            user.Password = password;
            user.Photos = new List<Photo>();
            return user;
        }

        public static bool SignUp(string credentials)
        {
            string username, password;
            GetCredentials(credentials, out username, out password);
            User user = CreateUser(username, password);
            lock (Users)
            {
                if (!Users.Contains(user))
                {
                    Users.Add(user);
                    return true;
                }
                else
                    throw new UserAlreadyExistException();
            }
        }
        public static bool DeleteUser(string username)
        {
            bool ret = true;

            try
            {
                lock (Users)
                {
                    for (int i = 0; i < Users.Count; i++)
                    {
                        User user = Users[i];
                        if (user.Username.Equals(username))
                        {
                            Users.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error al eliminar usuario");
                ret = false;
            }
            return ret;
        }
        public static bool ModifyUser(string username, string newCredentials)
        {
            string newUsername, newPassword;
            GetCredentials(newCredentials, out newUsername, out newPassword);

            bool ret = false;
            lock (Users)
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    User user = Users[i];
                    if (user.Username.Equals(username))
                    {
                        user.Username = newUsername;
                        user.Password = newPassword;
                        ret = true;
                    }
                }
            }
            return ret;
        }
    }
}
