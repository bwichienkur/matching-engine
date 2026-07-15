using EDDY.IS.FormsEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class FileReaderService : IFileReaderService
    {
        public string ReadAllTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
