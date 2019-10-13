using System;
using System.Collections.Generic;
using System.Text;
using ComplexFile.Core.Stream;

namespace ComplexFile.Core.Services.Builders
{
    interface IComplexFileBuilder
    {
        void CreateEmptyFile();
        IComplexFileStream ConfigureToComplexFile();
    }
}
