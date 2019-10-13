using System;
using System.Collections.Generic;
using System.Text;

namespace ComplexFile.Core.Services.Validators
{
    public interface IComplexFileValidator
    {
        bool InvalidPath(string path);
    }
}
