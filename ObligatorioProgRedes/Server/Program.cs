﻿using Common;
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

            string mensaje = "Hola bolso hoy te vinimo a ver";

            Header header = new Header(HeaderConstants.Request,1, mensaje.Length);

            var codedMessge = DataSend.GenMenssage(mensaje, header);

            Console.WriteLine(DataSend.DecodeMessage(codedMessge));

        }
    }
}
