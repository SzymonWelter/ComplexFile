using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexStorage
{
    public interface IBlockFactory
    {
        IBlock CreateBlock(byte[] block);
        byte[] CreateBlock(IBlock block);
    }

    public class BlockFactory : IBlockFactory
    {
        private BlockFactory() { 
        }

        private static BlockFactory instance = new BlockFactory();
        public static BlockFactory Instance => instance;

        private ICFSettings settings;

        public IBlock CreateBlock(byte[] block)
        {
            settings = ComplexFile.Settings;
            if (block[0] == (byte)0)
            {
                var hf = HeaderFactory.Instance;
                var fileHeaders = hf.CreateHeaders(
                        block.Skip(4).Take(settings.BlockSize - 8).ToArray()
                    );
                return new TOC()
                {
                    Type = 0,
                    Next = BitConverter.ToInt32(block, settings.BlockSize - 4),
                    FileHeaders = fileHeaders
                };
            }
            return new SystemFile()
            {
                Type = BitConverter.ToInt32(block, 0),
                Data = block.Skip(4).Take(settings.DataSize).ToArray(),
                Next = BitConverter.ToInt32(block, settings.DataSize + 4)
            };
        }
        public byte[] CreateBlock(IBlock block)
        {
            List<byte> byteList = new List<byte>();
            if (block.Type == 0)
            {
                var toc = (TOC)block;
                byteList.AddRange(BitConverter.GetBytes(toc.Type));
                foreach (FileHeader fileHeader in toc.FileHeaders)
                    byteList.AddRange(fileHeader.ToBytes());
                for (int index = 0; index < settings.HeadersNumber - toc.FileHeaders.Count; index++)
                    byteList.AddRange(new byte[settings.HeaderSize]);
                byteList.AddRange(BitConverter.GetBytes(toc.Next));
                return byteList.ToArray();
            }
            var file = (SystemFile)block;
            byteList.AddRange(BitConverter.GetBytes(file.Type));
            byteList.AddRange(file.Data);
            byteList.AddRange(BitConverter.GetBytes(file.Next));
            return byteList.ToArray();

        }
    }
    public class HeaderFactory
    {
        private HeaderFactory() { }

        private static HeaderFactory instance = new HeaderFactory();
        public static HeaderFactory Instance => instance;
        private ICFSettings settings = ComplexFile.Settings;
        public FileHeader CreateHeader(byte[] header)
        {
            int blockNumber = BitConverter.ToInt32(header, settings.IdSize + settings.NameSize);
            if (blockNumber == 0)
                return null;
            string id = Encoding.ASCII.GetString((header).Take(settings.IdSize).ToArray());
            string name = Encoding.ASCII.GetString(header.Skip(settings.IdSize).Take(FileHeader.NameSize).ToArray());
            int size = BitConverter.ToInt32(header, settings.IdSize + FileHeader.NameSize + 4);
            return new FileHeader()
            {
                BlockNumber = blockNumber,
                Id = id,
                Name = name,
                Size = size
            };
        }

        public List<FileHeader> CreateHeaders(byte[] headers)
        {
            var fileHeaderList = new List<FileHeader>();
            for (int index = 0; index < settings.HeadersNumber; index++)
            {
                var fileHeader = CreateHeader(
                    headers.Skip(index * settings.HeaderSize).Take(settings.HeaderSize).ToArray()
                );
                if (fileHeader == null)
                    return fileHeaderList;
                fileHeaderList.Add(fileHeader);
            }
            return fileHeaderList;
        }

        public byte[] CreateBlock(FileHeader fileHeader)
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(fileHeader.Id));
            byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(fileHeader.Name));
            byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(fileHeader.BlockNumber));
            byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(fileHeader.Size));
            return byteList.ToArray();
        }

        public FileHeader CreateDirectory(string name) =>
            new FileHeader{
                Id = "0" + Utilities.GenerateId(),
                Name = name + new string(char.MinValue, ComplexFile.Settings.NameSize - name.Length),
                Size = 0
            };        
        
        public FileHeader CreateFile(string name) =>
            new FileHeader{
                Id = "1" + Utilities.GenerateId(),
                Name = name + new string(char.MinValue, ComplexFile.Settings.NameSize - name.Length),
                Size = 0
            };
    }
}