using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ComplexFile.Core.Validation
{
    public class ComplexFilePathValidation : IPathValidation
    {
        public string Path { get; }

        public ComplexFilePathValidation(string path)
        {
            Path = path;
        }
        public bool ValidPath()
        {
            return Regex.IsMatch(Path, "/(.*?).cxfl/");
        }
    }
}
