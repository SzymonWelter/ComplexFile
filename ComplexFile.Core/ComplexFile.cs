using ComplexFile.Core.Exceptions;
using ComplexFile.Core.Validation;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComplexFile.Core
{
    public class ComplexFile : Stream
    {
        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get; }

        public ComplexFile(string path)
        {
            ValidPath(path);
        }

        private static void ValidPath(string path)
        {
            var vp = new ComplexFilePathValidation(path);
            if (!vp.ValidPath())
            {
                throw new WrongPathException(vp.Path);
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
