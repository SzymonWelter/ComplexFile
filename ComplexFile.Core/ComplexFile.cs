using ComplexFile.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComplexFile.Core
{
    public class ComplexFile
    {
        private readonly ComplexFileConfiguration _complexFileConfiguration;
        private string path;

        public ComplexFile(string path, ComplexFileConfiguration configuration)
        {
        }

        public ComplexFile(ComplexFileConfiguration complexFileConfiguration)
        {
            _complexFileConfiguration = complexFileConfiguration;
        }

        public ComplexFile(string path)
        {
            this.path = path;
        }

        public ComplexFileStream Create()
        {
            throw new NotImplementedException();
        }
    }
}
