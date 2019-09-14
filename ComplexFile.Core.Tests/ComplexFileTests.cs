using System;
using Xunit;
using ComplexFile.Core.Exceptions;

namespace ComplexFile.Core.Tests
{
    public class ComplexFileTests
    {
        [Fact]
        public void IfPassedPathWithWrongExtension_ThenThrowException()
        {
            var path = @"C:\Program Files\myfile.exe";

            Func<ComplexFile> cf = () => new ComplexFile(path);

            Assert.Throws<WrongPathException>(cf.Invoke);
        }

        [Fact]
        public void IfPassedPathWithoutExtension_ThenThrowException()
        {
            var path = @"C:\Program Files\Microsoft";

            Func<ComplexFile> cf = () => new ComplexFile(path);

            Assert.Throws<WrongPathException>(cf.Invoke);
        }
    }
}
