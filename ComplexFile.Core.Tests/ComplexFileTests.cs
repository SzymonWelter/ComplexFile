using System;
using Xunit;
using System.IO;
using NSubstitute;
using ComplexFile.Core.Configuration;
namespace ComplexFile.Core.Tests
{
    public class ComplexFileTests
    {
        [Fact]
        public void IfInitializeComplexFileWithPathAndWithoutConfiguration_ThenFileShouldBeCreated()
        {
            var path = @"ComplexFile.cplf";
            var complexFile = new ComplexFile(path);

            var complexFileStream = complexFile.Create();

            Assert.True(File.Exists(path));
        }

        [Fact]
        public void IfInitializeComplexFileWithCustomConfigurationWithPath_ThenFileShouldBeCreated()
        {
            var path = @"ComplexFile.cplf";
            var configuration = Substitute.For<IComplexFileConfiguration>();
            var complexFile = new ComplexFile(path, configuration);

            var complexFileStream = complexFile.Create();

            Assert.True(File.Exists(path));
        }
    }
}
