using ComplexFile.Core.Configuration;
using ComplexFile.Core.Services.Builders;
using ComplexFile.Core.Services.Informants;
using ComplexFile.Core.Stream;

namespace ComplexFile.Core
{
    public class ComplexFile
    {
        private readonly IComplexFileConfiguration _configuration;
        private readonly IComplexFileInfo _info;

        public ComplexFile(string path) : this(path, new DefaultComplexFileConfiguration())
        { }

        public ComplexFile(string path, IComplexFileConfiguration configuration)
        {
            _info.Path = path;
            _configuration = configuration;
        }

        public IComplexFileStream Create()
        {
            var complexFileBuilder = new ComplexFileBuilder(_info);
            complexFileBuilder.CreateEmptyFile();
            var fileStream = complexFileBuilder.ConfigureToComplexFile();
            
            return fileStream;
        }        
    }
}
