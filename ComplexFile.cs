// Decompiled with JetBrains decompiler
// Type: ComplexStorage.ComplexFile
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ComplexStorage
{
  public class ComplexFile : FileStream
  {
    public byte[] Fib { get; set; }

    public static int BlockSize
    {
      get
      {
        return 512;
      }
    }

    public int BlockNumber
    {
      get
      {
        return (int) (this.Length - 4L) / ComplexFile.BlockSize;
      }
    }

    public string RootName
    {
      get
      {
        return "root";
      }
    }

    private ComplexFile(string path, FileMode mode)
      : base(path, mode)
    {
      this.Fib = new byte[4]
      {
        (byte) 83,
        (byte) 90,
        (byte) 87,
        (byte) 27
      };
    }

    public static ComplexFile Create(string name)
    {
      ComplexFile complexFile = new ComplexFile(name, FileMode.Create);
      List<byte> byteList = new List<byte>();
      byteList.AddRange(complexFile.Fib);
      byteList.AddRange(TOC.CreateEmpty());
      complexFile.Write(byteList.ToArray(), 0, byteList.Count);
      complexFile.Close();
      return Open(name);
    }

    public static ComplexFile OpenOrCreate(string name)
    {
      if (!File.Exists(name))
        return Create(name);
      return Open(name);
    }

    public static ComplexFile Open(string name)
    {
      return new ComplexFile(name, FileMode.Open);
    }

    public string CreateDirectory(string name, string localizationId, string id = "")
    {
      if (id == "")
        id = this.GenerateId("0");
      try
      {
        FileHeader fileHeader = new FileHeader()
        {
          Id = id,
          Name = name + new string(char.MinValue, FileHeader.NameSize - name.Length),
          Size = 0
        };
        TOC tocForHeader = this.GetTOCForHeader(fileHeader, localizationId);
        fileHeader.BlockNumber = this.BlockNumber;
        WriteNewTOC();
        tocForHeader.AddFileHeader(fileHeader);
        Position = (long) (tocForHeader.BlockNumber * BlockSize + 4);
        Write(tocForHeader.ToBytes(), 0, BlockSize);
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
      Enumerable.Range(65, 26).Select<int, string>((Func<int, string>) (e => ((char) e).ToString())).Concat<string>(Enumerable.Range(97, 26).Select<int, string>((Func<int, string>) (e => ((char) e).ToString()))).Concat<string>(Enumerable.Range(0, 10).Select<int, string>((Func<int, string>) (e => e.ToString()))).OrderBy<string, Guid>((Func<string, Guid>) (e => Guid.NewGuid())).Take<string>(FileHeader.IdSize - 1).ToList<string>().ForEach((Action<string>) (e => id.Append(e)));
      return type + (object) id;
    }

    public string CreateFile(Stream stream, string name, string localizationId, string id = "")
    {
      if (id == "")
        id = this.GenerateId("1");
      FileHeader fileHeader = new FileHeader()
      {
        Id = id,
        Name = name + new string(char.MinValue, FileHeader.NameSize - name.Length),
        Size = (int) stream.Length
      };
      TOC tocForHeader = this.GetTOCForHeader(fileHeader, localizationId);
      fileHeader.BlockNumber = this.BlockNumber;
      stream.Position = 0L;
      this.Seek(0L, SeekOrigin.End);
      byte[] buffer = new byte[ComplexFile.BlockSize - 8];
      for (int count = stream.Read(buffer, 0, ComplexFile.BlockSize - 8); count > 0; count = stream.Read(buffer, 0, ComplexFile.BlockSize - 8))
      {
        int blockNumber = this.BlockNumber + 1;
        if (count < ComplexFile.BlockSize - 8)
          blockNumber = 0;
        this.Write(SystemFile.ToBytes(buffer, count, blockNumber), 0, ComplexFile.BlockSize);
        buffer = new byte[ComplexFile.BlockSize - 8];
      }
      tocForHeader.AddFileHeader(fileHeader);
      this.Position = (long) (tocForHeader.BlockNumber * ComplexFile.BlockSize + 4);
      this.Write(tocForHeader.ToBytes(), 0, ComplexFile.BlockSize);
      return "The file was uploaded successfully";
    }

    public TOC GetTOCById(string id)
    {
      IBlock blockById = this.GetBlockById(id);
      return blockById.GetType() == typeof (TOC) ? (TOC) blockById : (TOC) null;
    }

    public SystemFile GetFileById(string id)
    {
      IBlock blockById = this.GetBlockById(id);
      return blockById.GetType() == typeof (SystemFile) ? (SystemFile) blockById : (SystemFile) null;
    }

    private IBlock GetBlockById(string id)
    {
      this.GetTOCs();
      if (id == this.RootName)
        return (IBlock) this.GetTOCByBlockNumber(0);
      Stack<int> intStack = new Stack<int>();
      intStack.Push(0);
      while (intStack.Count > 0)
      {
        TOC tocByBlockNumber = this.GetTOCByBlockNumber(intStack.Pop());
        if (tocByBlockNumber != null)
        {
          foreach (FileHeader fileHeader in tocByBlockNumber.FileHeaders)
          {
            if (fileHeader.Id == id)
              return this.GetBlockByNumber(fileHeader.BlockNumber);
            intStack.Push(fileHeader.BlockNumber);
          }
          for (; (uint) tocByBlockNumber.Next > 0U; tocByBlockNumber = this.GetTOCByBlockNumber(tocByBlockNumber.Next))
            intStack.Push(tocByBlockNumber.Next);
        }
      }
      return (IBlock) null;
    }

    public List<TOC> GetTOCs()
    {
      Stack<int> intStack = new Stack<int>();
      List<TOC> source = new List<TOC>();
      intStack.Push(0);
      while (intStack.Count > 0)
      {
        TOC tocByBlockNumber = this.GetTOCByBlockNumber(intStack.Pop());
        if (tocByBlockNumber != null)
        {
          source.Add(tocByBlockNumber);
          foreach (FileHeader fileHeader in tocByBlockNumber.FileHeaders)
            intStack.Push(fileHeader.BlockNumber);
          while ((uint) tocByBlockNumber.Next > 0U)
          {
            tocByBlockNumber = this.GetTOCByBlockNumber(tocByBlockNumber.Next);
            foreach (FileHeader fileHeader in tocByBlockNumber.FileHeaders)
              intStack.Push(fileHeader.BlockNumber);
            source.Add(tocByBlockNumber);
          }
        }
      }
      return source.OrderBy<TOC, int>((Func<TOC, int>) (x => x.BlockNumber)).ToList<TOC>();
    }

    public List<FileHeader> GetDirectoryById(string localizationId)
    {
      TOC toc = this.GetTOCById(localizationId);
      List<FileHeader> fileHeaders = toc.FileHeaders;
      while ((uint) toc.Next > 0U)
      {
        toc = this.GetTOCByBlockNumber(toc.Next);
        if (toc.GetType() == typeof (TOC))
          fileHeaders.AddRange((IEnumerable<FileHeader>) toc.FileHeaders);
      }
      return fileHeaders.OrderBy<FileHeader, string>((Func<FileHeader, string>) (x => x.Name)).ToList<FileHeader>();
    }

    public FileHeader GetParentHeader(string id)
    {
      return this.GetFileHeaderByBlockNumber(this.GetTOCUpper(id).BlockNumber);
    }

    private FileHeader GetFileHeaderByBlockNumber(int blockNumber)
    {
      if (blockNumber == 0)
        return new FileHeader() { Id = this.RootName };
      foreach (TOC toC in this.GetTOCs())
      {
        foreach (FileHeader fileHeader in toC.FileHeaders)
        {
          if (fileHeader.BlockNumber == blockNumber)
            return fileHeader;
        }
      }
      return (FileHeader) null;
    }

    private TOC GetTOCUpper(string id)
    {
      if (id == this.RootName)
        return this.GetTOCByBlockNumber(0);
      TOC toc1 = (TOC) null;
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
          if (toc2.Next == toc1.BlockNumber && (uint) toc1.BlockNumber > 0U)
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
      return (TOC) null;
    }

    public TOC GetTOCByBlockNumber(int blockNumber)
    {
      IBlock blockByNumber = this.GetBlockByNumber(blockNumber);
      return blockByNumber.GetType() == typeof (TOC) ? (TOC) blockByNumber : (TOC) null;
    }

    public SystemFile GetFileByBlockNumber(int blockNumber)
    {
      IBlock blockByNumber = this.GetBlockByNumber(blockNumber);
      return blockByNumber.GetType() == typeof (SystemFile) ? (SystemFile) blockByNumber : (SystemFile) null;
    }

    public IBlock GetBlockByNumber(int blockNumber)
    {
      byte[] numArray = new byte[ComplexFile.BlockSize];
      this.Position = (long) (blockNumber * ComplexFile.BlockSize + 4);
      this.Read(numArray, 0, ComplexFile.BlockSize);
      if (numArray[0] == (byte) 0)
      {
        TOC toc = TOC.Create(numArray);
        toc.BlockNumber = blockNumber;
        return (IBlock) toc;
      }
      SystemFile systemFile = SystemFile.Create(numArray);
      systemFile.BlockNumber = blockNumber;
      return (IBlock) systemFile;
    }

    public FileHeader GetFileHeaderById(string id)
    {
      foreach (TOC toC in this.GetTOCs())
      {
        FileHeader fileHeaderById = toC.GetFileHeaderById(id);
        if (fileHeaderById != null)
          return fileHeaderById;
      }
      return (FileHeader) null;
    }

    public byte[] GetFile(string id)
    {
      FileHeader fileHeaderById = this.GetFileHeaderById(id);
      SystemFile blockByNumber = (SystemFile) this.GetBlockByNumber(fileHeaderById.BlockNumber);
      List<byte> source = new List<byte>((IEnumerable<byte>) blockByNumber.Data);
      while ((uint) blockByNumber.Next > 0U)
      {
        blockByNumber = (SystemFile) this.GetBlockByNumber(blockByNumber.Next);
        source.AddRange((IEnumerable<byte>) blockByNumber.Data);
      }
      return source.Take<byte>(fileHeaderById.Size).ToArray<byte>();
    }

    private void WriteNewTOC()
    {
      this.Seek(0L, SeekOrigin.End);
      this.Write(TOC.CreateEmpty(), 0, ComplexFile.BlockSize);
    }

    private TOC GetTOCForHeader(FileHeader fileHeader, string localizationId)
    {
      TOC toc = this.GetTOCById(localizationId);
      if (toc.ContainsFileHeader(fileHeader.Id))
        throw new Exception("That path exists already!");
      for (; toc.Next != 0 && toc.FileHeaders.Count == TOC.FileHeaderNumber; toc = this.GetTOCByBlockNumber(toc.Next))
      {
        if (toc.ContainsFileHeader(fileHeader.Id))
          throw new Exception("That path exists already!");
      }
      if (toc.FileHeaders.Count == TOC.FileHeaderNumber)
      {
        this.WriteNewTOC();
        this.Position = (long) ((toc.BlockNumber + 1) * ComplexFile.BlockSize);
        this.Write(BitConverter.GetBytes(this.BlockNumber - 1), 0, 4);
        toc = new TOC()
        {
          BlockNumber = this.BlockNumber - 1
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
