using Common.Interfaces;
using Domain;
using Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileSenderHandler : IFileSenderHandler
    {
        public static long FileSegmentSize = 32768;
        public byte[] Read(string path, long offset, int length)
        {
            var data = new byte[length];

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Position = offset;
                var byteRecived = 0;
                while(byteRecived < length)
                {
                    var recived = fs.Read(data, byteRecived, length - byteRecived);
                    if(recived == 0)
                    {
                        throw new ReadFileException();
                    }
                    byteRecived += recived;

                }
            }

            return data;
        }

        public void Write(string fileName, byte[] data)
        {
            if (File.Exists(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Append))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            else
            {
                using(var fs = new FileStream(fileName, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
        }
    }
}
