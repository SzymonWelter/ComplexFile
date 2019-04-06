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

    public List<FileHeader> FileHeaders { get; set; }

    public int Type { get; set; }

    public int Next { get; set; }

    public int BlockNumber { get; set; }

    public bool AddFileHeader(FileHeader newHeader)
    {
      if (FileHeaders.Count == ComplexFile.Settings.HeadersNumber)
        return false;
      FileHeaders.Add(newHeader);
      return true;
    }

    public FileHeader GetFileHeaderById(string id)
    {
      foreach (FileHeader fileHeader in FileHeaders)
      {
        if (fileHeader.Id == id)
          return fileHeader;
      }
      return null;
    }

    public bool ContainsFileHeader(string id)
    {
      foreach (var fileHeader in FileHeaders)
      {
        if (fileHeader.Id == id)
          return true;
      }
      return false;
    }
  }
}
