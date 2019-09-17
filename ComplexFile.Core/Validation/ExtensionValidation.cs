using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ComplexFile.Core.Validation
{
    public class ExtensionValidation : IPathValidationMember
    {
        private string _extension;
        private IPathValidationMember _next;
        public string Path { get; private set; }

        public ExtensionValidation(string extension)
        {
            _extension = extension;
        }

        public void SetNext(IPathValidationMember next)
        {
            _next = next;
            Path = next.Path;
        }

        public bool Handle()
        {
           if (IsInvalid() || _next == null)
                return false;
           return _next.Handle();            
        }

        private bool IsInvalid()
        {
            return !Regex.IsMatch(Path, $"/(.*?).{_extension}/");
        }
    }
}
