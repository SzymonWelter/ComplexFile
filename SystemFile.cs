// Decompiled with JetBrains decompiler
// Type: ComplexStorage.SystemFile
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace ComplexStorage
{
  public class SystemFile : IBlock
  {
    public static int DataSize
    {
      get
      {
        return ComplexFile.BlockSize - 8;
      }
    }

    public int Type { get; set; }

    public byte[] Data { get; set; }

    public int Next { get; set; }

    public int BlockNumber { get; set; }

    public static SystemFile Create(byte[] bytes)
    {
      return new SystemFile()
      {
        Type = BitConverter.ToInt32(bytes, 0),
        Next = BitConverter.ToInt32(bytes, SystemFile.DataSize + 4),
        Data = ((IEnumerable<byte>) bytes).Skip<byte>(4).Take<byte>(SystemFile.DataSize).ToArray<byte>()
      };
    }

    public byte[] ToBytes()
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.Type));
      byteList.AddRange((IEnumerable<byte>) this.Data);
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(this.Next));
      return byteList.ToArray();
    }

    public static byte[] ToBytes(byte[] buffer, int count, int blockNumber)
    {
      List<byte> byteList = new List<byte>();
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(1));
      byteList.AddRange(((IEnumerable<byte>) buffer).Take<byte>(count));
      byteList.AddRange((IEnumerable<byte>) new byte[ComplexFile.BlockSize - 8 - count]);
      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(blockNumber));
      return byteList.ToArray();
    }
  }
}
