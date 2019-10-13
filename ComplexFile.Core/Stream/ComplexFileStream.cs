﻿using System;
using System.IO;

namespace ComplexFile.Core.Stream
{
    public class ComplexFileStream : FileStream
    {
        public ComplexFileStream(string path, FileMode mode) : base(path, mode)
        {
        }
    }
}