using System;
using System.Collections.Generic;
using System.Text;

namespace ComplexFile.Core.Exceptions
{
    public class WrongPathException : Exception
    {
        public WrongPathException(string path) : base($"Passed path: {path} is wrong")
        {

        }
    }
}
