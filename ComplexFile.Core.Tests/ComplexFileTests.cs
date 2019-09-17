using System;
using Xunit;
using ComplexFile.Core.Exceptions;
using System.IO;

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

        [Fact]
        public void IfPassedPathWhichIsNotAllowedForUser_ThenThrowException()
        {
            var path = @"C:\Windows\cplfile.cxfl";

            Func<ComplexFile> cf = () => new ComplexFile(path);

            Assert.Throws<WrongPathException>(cf.Invoke);
        }
        [Fact]
        public void IfPassedPathIsCorrectAndFileDontExists_ThenComplexFileShouldBeCreated()
        {
            var path = @"complexFile.cxfl";

            using (_ = new ComplexFile(path)) { } ;

            Assert.True(File.Exists(path));
        }

        [Fact]
        public void IfPassedPathIsCorrectAndFileExists_ThenComplexFileShouldStoreNameOfFile()
        {
            var path = @"complexFile.cxfl";
            using (_ = File.Create(path)) { } ;
            using (var complexFile = new ComplexFile(path)) {

                Assert.Equal(Path.GetFileName(path), Path.GetFileName(complexFile.Name));
            };
        }
    }
}
