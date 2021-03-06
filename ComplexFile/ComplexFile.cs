﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ComplexStorage
{
    public class ComplexFile : FileStream
    {
        public static ICFSettings Settings; 

        private IBlockFactory blockFactory;
        private HeaderFactory headerFactory;
        private List<TOC> TOCs;

        /*
        ** Actual number of blocks in ComplexFile
        */
        public int BlockNumber => (int) (Length / Settings.BlockSize);

        private ComplexFile(string path, FileMode mode)
          : base(path, mode)
        {
            Settings = DefaultSettings.Instance; //TODO Load settings from zero block
            blockFactory = BlockFactory.Instance;
            headerFactory = HeaderFactory.Instance;
            TOCs = GetTOCs();
        }

        /*
        ** Create ComplexFile with specified name
        */
        public static ComplexFile Create(string path, ICFSettings settings)
        {
            ComplexFile complexFile = new ComplexFile(path, FileMode.Create);
            complexFile.Write(Utilities.ToBytes(Settings), 0, Settings.BlockSize);
            complexFile.Write(new byte[Settings.BlockSize], 0, Settings.BlockSize);
            complexFile.Close();
            return Open(path);
        }

        public static ComplexFile OpenOrCreate(string path)
        {
            if (!File.Exists(path))
                return Create(path, null);
            return Open(path);
        }

        public static ComplexFile Open(string path)
        {
            return new ComplexFile(path, FileMode.Open);
        }

        public string CreateDirectory(string name, string localizationId)
        {
            try
            {
                var fileHeader = headerFactory.CreateDirectory(name);
                TOC tocForHeader = GetTOCForHeader(fileHeader, localizationId);
                fileHeader.BlockNumber = BlockNumber;
                WriteNewTOC();
                tocForHeader.AddFileHeader(fileHeader);
                Position = (tocForHeader.BlockNumber * Settings.BlockSize);
                Write(blockFactory.CreateBlock(tocForHeader), 0, Settings.BlockSize);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "The directory was created successfully";
        }

        public string GenerateId(string type)
        {
            StringBuilder id = new StringBuilder();
            Enumerable.Range(65, 26).Select<int, string>((Func<int, string>)(e => ((char)e).ToString())).Concat<string>(Enumerable.Range(97, 26).Select<int, string>((Func<int, string>)(e => ((char)e).ToString()))).Concat<string>(Enumerable.Range(0, 10).Select<int, string>((Func<int, string>)(e => e.ToString()))).OrderBy<string, Guid>((Func<string, Guid>)(e => Guid.NewGuid())).Take<string>(FileHeader.IdSize - 1).ToList<string>().ForEach((Action<string>)(e => id.Append(e)));
            return type + id.ToString();
        }

        public string CreateFile(Stream stream, string name, string localizationId)
        {
            var fileHeader = headerFactory.CreateFile(name);
            TOC tocForHeader = GetTOCForHeader(fileHeader, localizationId);
            fileHeader.BlockNumber = BlockNumber;
            stream.Position = 0;
            Seek(0, SeekOrigin.End);
            byte[] buffer = new byte[Settings.BlockSize - 8];
            for (int count = stream.Read(buffer, 0, Settings.BlockSize - 8); count > 0; count = stream.Read(buffer, 0, Settings.BlockSize - 8))
            {
                int blockNumber = BlockNumber + 1;
                if (count < Settings.BlockSize - 8)
                    blockNumber = 0;
                Write(SystemFile.ToBytes(buffer, count, blockNumber), 0, Settings.BlockSize);
                buffer = new byte[Settings.BlockSize - 8];
            }
            tocForHeader.AddFileHeader(fileHeader);
            Position = (long)(tocForHeader.BlockNumber * Settings.BlockSize + 4);
            Write(blockFactory.CreateBlock(tocForHeader), 0, Settings.BlockSize);
            return "The file was uploaded successfully";
        }

        public TOC GetTOCById(string id) => (TOC)GetBlockById(id);

        public SystemFile GetFileById(string id) => (SystemFile)GetBlockById(id);

        private IBlock GetBlockById(string id)
        {
            if (id == Settings.RootName)
                return GetTOCByBlockNumber(1);
            Stack<int> intStack = new Stack<int>();
            intStack.Push(1);
            while (intStack.Count > 0)
            {
                TOC tocByBlockNumber = GetTOCByBlockNumber(intStack.Pop());
                if (tocByBlockNumber != null)
                {
                    foreach (FileHeader fileHeader in tocByBlockNumber.FileHeaders)
                    {
                        if (fileHeader.Id == id)
                            return GetBlockByNumber(fileHeader.BlockNumber);
                        intStack.Push(fileHeader.BlockNumber);
                    }
                    while (tocByBlockNumber.Next > 0)
                    {
                        intStack.Push(tocByBlockNumber.Next);
                        tocByBlockNumber = GetTOCByBlockNumber(tocByBlockNumber.Next);
                    }
                }
            }
            return null;
        }

        public List<TOC> GetTOCs()
        {
            Stack<int> blockNumber = new Stack<int>();
            List<TOC> source = new List<TOC>();
            blockNumber.Push(0);
            while (blockNumber.Count > 0)
            {
                TOC tocByBlockNumber = GetTOCByBlockNumber(blockNumber.Pop());
                if (tocByBlockNumber != null)
                {
                    source.Add(tocByBlockNumber);
                    foreach (FileHeader fileHeader in tocByBlockNumber.FileHeaders)
                        blockNumber.Push(fileHeader.BlockNumber);
                    while (tocByBlockNumber.Next > 0)
                    {
                        tocByBlockNumber = GetTOCByBlockNumber(tocByBlockNumber.Next);
                        foreach (FileHeader fileHeader in tocByBlockNumber.FileHeaders)
                            blockNumber.Push(fileHeader.BlockNumber);
                        source.Add(tocByBlockNumber);
                    }
                }
            }
            return source.OrderBy<TOC, int>((Func<TOC, int>)(x => x.BlockNumber)).ToList<TOC>();
        }

        public List<FileHeader> GetDirectoryById(string localizationId)
        {
            TOC toc = this.GetTOCById(localizationId);
            List<FileHeader> fileHeaders = toc.FileHeaders;
            while ((uint)toc.Next > 0U)
            {
                toc = this.GetTOCByBlockNumber(toc.Next);
                if (toc.GetType() == typeof(TOC))
                    fileHeaders.AddRange((IEnumerable<FileHeader>)toc.FileHeaders);
            }
            return fileHeaders.OrderBy<FileHeader, string>((Func<FileHeader, string>)(x => x.Name)).ToList<FileHeader>();
        }

        public FileHeader GetParentHeader(string id)
        {
            return GetFileHeaderByBlockNumber(GetTOCUpper(id).BlockNumber);
        }

        private FileHeader GetFileHeaderByBlockNumber(int blockNumber)
        {
            if (blockNumber == 0)
                return new FileHeader() { Id = Settings.RootName };
            foreach (TOC toC in this.GetTOCs())
            {
                foreach (FileHeader fileHeader in toC.FileHeaders)
                {
                    if (fileHeader.BlockNumber == blockNumber)
                        return fileHeader;
                }
            }
            return (FileHeader)null;
        }

        private TOC GetTOCUpper(string id)
        {
            if (id == Settings.RootName)
                return this.GetTOCByBlockNumber(0);
            TOC toc1 = (TOC)null;
            List<TOC> toCs = this.GetTOCs();
            foreach (TOC toc2 in toCs)
            {
                foreach (FileHeader fileHeader in toc2.FileHeaders)
                {
                    if (fileHeader.Id == id)
                    {
                        toc1 = toc2;
                        break;
                    }
                }
                if (toc1 != null)
                    break;
            }
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (TOC toc2 in toCs)
                {
                    if (toc2.Next == toc1.BlockNumber && (uint)toc1.BlockNumber > 0U)
                    {
                        flag = true;
                        toc1 = toc2;
                        break;
                    }
                }
            }
            return toc1;
        }

        private TOC GetTOCUpper(int blockNumber)
        {
            foreach (TOC toC in this.GetTOCs())
            {
                foreach (FileHeader fileHeader in toC.FileHeaders)
                {
                    if (fileHeader.BlockNumber == blockNumber)
                        return toC;
                }
            }
            return (TOC)null;
        }

        public TOC GetTOCByBlockNumber(int blockNumber)
        {
            var toc = (TOC)GetBlockByNumber(blockNumber);
            toc.BlockNumber = blockNumber;
            return toc;
        }

        public SystemFile GetFileByBlockNumber(int blockNumber)
        {
            var sf = (SystemFile)GetBlockByNumber(blockNumber);
            sf.BlockNumber = blockNumber;
            return sf;
        }

        public IBlock GetBlockByNumber(int blockNumber)
        {
            byte[] blockBytes = new byte[Settings.BlockSize];
            Position = (blockNumber * Settings.BlockSize);
            Read(blockBytes, 0, Settings.BlockSize);
            return blockFactory.CreateBlock(blockBytes);
        }

        public FileHeader GetFileHeaderById(string id)
        {
            foreach (TOC toC in this.GetTOCs())
            {
                FileHeader fileHeaderById = toC.GetFileHeaderById(id);
                if (fileHeaderById != null)
                    return fileHeaderById;
            }
            return (FileHeader)null;
        }

        public byte[] GetFile(string id)
        {
            FileHeader fileHeaderById = this.GetFileHeaderById(id);
            SystemFile blockByNumber = (SystemFile)this.GetBlockByNumber(fileHeaderById.BlockNumber);
            List<byte> source = new List<byte>((IEnumerable<byte>)blockByNumber.Data);
            while ((uint)blockByNumber.Next > 0U)
            {
                blockByNumber = (SystemFile)this.GetBlockByNumber(blockByNumber.Next);
                source.AddRange((IEnumerable<byte>)blockByNumber.Data);
            }
            return source.Take<byte>(fileHeaderById.Size).ToArray<byte>();
        }

        private void WriteNewTOC()
        {
            Seek(0, SeekOrigin.End);
            Write(new byte[Settings.BlockSize], 0, Settings.BlockSize);
        }

        private TOC GetTOCForHeader(FileHeader fileHeader, string localizationId)
        {
            TOC toc = GetTOCById(localizationId);
            if (toc.ContainsFileHeader(fileHeader.Id))
                throw new Exception("That path exists already!");
            while (toc.Next != 0 && toc.FileHeaders.Count == Settings.HeadersNumber )
            {
                if (toc.ContainsFileHeader(fileHeader.Id))
                    throw new Exception("That path exists already!");
                toc = GetTOCByBlockNumber(toc.Next);
            }
            if (toc.FileHeaders.Count == Settings.HeadersNumber)
            {
                WriteNewTOC();
                Position = (toc.BlockNumber + 1) * Settings.BlockSize - 4;
                Write(BitConverter.GetBytes(BlockNumber - 1), 0, 4);
                toc = new TOC()
                {
                    BlockNumber = BlockNumber - 1
                };
            }
            return toc;
        }

        public override void Close()
        {
            base.Close();
        }
    }
}
