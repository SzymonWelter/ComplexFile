using ComplexFile.Core.Exceptions;
using ComplexFile.Core.Validation;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComplexFile.Core
{
    public class ComplexFile
    {
        public string Name => Path.GetFileName(_path);
        private string _path;

        public ComplexFile(string path)
        {
            ValidPath(path);
            _path = path;
        }

        private static void ValidPath(string path)
        {

        }
    }
}
