// Decompiled with JetBrains decompiler
// Type: ComplexStorage.FileHeader
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexStorage
{
  public class FileHeader
  {
    public string Name { get; set; }

    public string Id { get; set; }

    public int BlockNumber { get; set; }

    public int Size { get; set; }

    public static int HeaderSize
    {
      get
      {
        return (ComplexFile.BlockSize - 8) / TOC.FileHeaderNumber;
      }
    }

    public static int IdSize
    {
      get
      {
        return 18;
      }
    }

    public static int NameSize
    {
      get
      {
        return 100;
      }
    }

    public static FileHeader Create(byte[] bytes)
    {
      int int32_1 = BitConverter.ToInt32(bytes, FileHeader.IdSize + FileHeader.NameSize);
      if (int32_1 == 0)
        return (FileHeader) null;
      string str1 = Encoding.ASCII.GetString(((IEnumerable<byte>) bytes).Take<byte>(FileHeader.IdSize).ToArray<byte>());
      string str2 = Encoding.ASCII.GetString(((IEnumerable<byte>) bytes).Skip<byte>(FileHeader.IdSize).Take<byte>(FileHeader.NameSize).ToArray<byte>());
      int int32_2 = BitConverter.ToInt32(bytes, FileHeader.IdSize + FileHeader.NameSize + 4);
      return new FileHeader()
      {
        BlockNumber = int32_1,
        Id = str1,
        Name = str2,
        Size = int32_2
      };
    }

    public byte[] ToBytes()
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(this.Id));
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(this.Name));
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.BlockNumber));
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.Size));
      return byteList.ToArray();
    }

    public static byte[] Empty()
    {
      return new byte[FileHeader.HeaderSize];
    }
  }
}
