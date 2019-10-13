using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ComplexFile.Core.Services.Informants;
using ComplexFile.Core.Services.Validators;
using ComplexFile.Core.Stream;

namespace ComplexFile.Core.Services.Builders
{
    public class ComplexFileBuilder : IComplexFileBuilder
    {
        private IComplexFileInfo _info;
        private readonly IComplexFileValidator _validator;

        public ComplexFileBuilder(IComplexFileInfo info)
        {
            _info = info;
            _validator = new ComplexFileValidator();
        }

        public void CreateEmptyFile()
        {
            CreateEmptyFile(_info.Path);
        }

        public void CreateEmptyFile(string path)
        {
            if (_validator.InvalidPath(path))
                throw new Exception("Invalid path");

            File.Create(path).Dispose();
        }

        public IComplexFileStream ConfigureToComplexFile()
        {
            throw new NotImplementedException();
        }
    }
}
