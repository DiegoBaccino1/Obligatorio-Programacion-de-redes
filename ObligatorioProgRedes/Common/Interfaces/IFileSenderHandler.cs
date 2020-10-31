using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IFileSenderHandler
    {
        byte[] Read(string path, long offset, int length);
        void Write(string fileName, byte[] data);
    }
}
