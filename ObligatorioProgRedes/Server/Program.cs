using Common;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

            Server server = new Server();

            server.StartServer();

            string mensaje = "Hola bolso hoy te vinimo a ver";

            Header header = new Header(HeaderConstants.Request,CommandConstants.Login, mensaje.Length);

            var codedMessge = DataTransfer.GenMenssage(mensaje, header);

            Console.WriteLine(DataTransfer.DecodeMessage(codedMessge));

        }
    }
}
