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
    public static int IdSize => 18;
    public static int NameSize => 100;

    public static FileHeader Create(byte[] bytes)
    {
      int headerSize = ComplexFile.Settings.HeaderSize;
      int blockNumber = BitConverter.ToInt32(bytes, headerSize + FileHeader.NameSize);
      if (blockNumber == 0)
        return null;
      string id = Encoding.ASCII.GetString((bytes).Take(headerSize).ToArray());
      string name = Encoding.ASCII.GetString(bytes.Skip(headerSize).Take(FileHeader.NameSize).ToArray());
      int size = BitConverter.ToInt32(bytes, headerSize + FileHeader.NameSize + 4);
      return new FileHeader()
      {
        BlockNumber = blockNumber,
        Id = id,
        Name = name,
        Size = size
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
  }
}
