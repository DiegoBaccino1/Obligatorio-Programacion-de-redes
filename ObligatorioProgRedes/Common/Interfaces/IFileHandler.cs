using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IFileHandler
    {
        bool FileExists(string path);
        string GetFileName(string path);
        long GetFileSize(string path);

    }
}
