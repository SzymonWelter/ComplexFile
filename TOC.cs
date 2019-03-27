// Decompiled with JetBrains decompiler
// Type: ComplexStorage.TOC
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComplexStorage
{
  public class TOC : IBlock
  {
    public TOC()
    {
      this.FileHeaders = new List<FileHeader>();
      this.Next = 0;
      this.Type = 0;
    }

    public List<FileHeader> FileHeaders { get; private set; }

    public int Type { get; set; }

    public int Next { get; set; }

    public static int FileHeaderNumber
    {
      get
      {
        return 4;
      }
    }

    public int BlockNumber { get; set; }

    public static byte[] CreateEmpty()
    {
      return new byte[ComplexFile.BlockSize];
    }

    public static TOC Create(byte[] toc)
    {
      int int32_1 = BitConverter.ToInt32(toc, 0);
      List<FileHeader> fileHeaders = TOC.GetFileHeaders(((IEnumerable<byte>) toc).Skip<byte>(4).Take<byte>(TOC.FileHeaderNumber * FileHeader.HeaderSize).ToArray<byte>());
      int int32_2 = BitConverter.ToInt32(toc, ComplexFile.BlockSize - 4);
      return new TOC()
      {
        Type = int32_1,
        Next = int32_2,
        FileHeaders = fileHeaders
      };
    }

    public byte[] ToBytes()
    {
      byte[] array = this.AppendBytesToArray(BitConverter.GetBytes(this.Type), new byte[0]);
      foreach (FileHeader fileHeader in this.FileHeaders)
        array = this.AppendBytesToArray(fileHeader.ToBytes(), array);
      for (int index = 0; index < TOC.FileHeaderNumber - this.FileHeaders.Count; ++index)
        array = this.AppendBytesToArray(FileHeader.Empty(), array);
      return this.AppendBytesToArray(BitConverter.GetBytes(this.Next), array);
    }

    public bool AddFileHeader(FileHeader newHeader)
    {
      if (this.FileHeaders.Count == TOC.FileHeaderNumber)
        return false;
      this.FileHeaders.Add(newHeader);
      return true;
    }

    public static List<FileHeader> GetFileHeaders(byte[] headers)
    {
      List<FileHeader> fileHeaderList = new List<FileHeader>();
      for (int index = 0; index < TOC.FileHeaderNumber; ++index)
      {
        FileHeader fileHeader = FileHeader.Create(((IEnumerable<byte>) headers).Skip<byte>(index * FileHeader.HeaderSize).Take<byte>(FileHeader.HeaderSize).ToArray<byte>());
        if (fileHeader == null)
          return fileHeaderList;
        fileHeaderList.Add(fileHeader);
      }
      return fileHeaderList;
    }

    public FileHeader GetFileHeaderById(string id)
    {
      foreach (FileHeader fileHeader in this.FileHeaders)
      {
        if (fileHeader.Id == id)
          return fileHeader;
      }
      return (FileHeader) null;
    }

    private byte[] AppendBytesToArray(byte[] sourceArray, byte[] destinationArray)
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange((IEnumerable<byte>) destinationArray);
      byteList.AddRange((IEnumerable<byte>) sourceArray);
      if (byteList.Count > ComplexFile.BlockSize)
        throw new IndexOutOfRangeException("Destination array is to short!");
      return byteList.ToArray();
    }

    public bool ContainsFileHeader(string id)
    {
      foreach (FileHeader fileHeader in this.FileHeaders)
      {
        if (fileHeader.Id == id)
          return true;
      }
      return false;
    }
  }
}
