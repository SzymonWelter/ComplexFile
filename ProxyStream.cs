// Decompiled with JetBrains decompiler
// Type: ComplexStorage.ProxyStream
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

using System;
using System.IO;

namespace ComplexStorage
{
  public class ProxyStream : Stream
  {
    private Stream cf;
    private ICFSettings settings;
    private readonly int startBlock;
    private readonly long size;

    public ProxyStream(ComplexFile cf, FileHeader fileHeader)
    {
      settings = ComplexFile.Settings;
      this.cf = (FileStream) cf;
      startBlock = fileHeader.BlockNumber;
      size = (long) fileHeader.Size;
      Position = 0;
    }

    public override bool CanRead => cf.CanRead;

    public override bool CanSeek => cf.CanSeek;
    public override bool CanWrite => cf.CanWrite;

    public override long Length => size;
    public override long Position
    {
      get
      {
        return (long) (((int) ((this.cf.Position - 4) / (long) settings.BlockSize) - this.startBlock) * settings.DataSize + ((int) ((this.cf.Position - 4L) % (long) settings.BlockSize) - 4));
      }
      set
      {
        if (value <= size && value >= 0)
          cf.Position = (long) (((int) (value / (long) settings.DataSize) + this.startBlock) * settings.BlockSize + 4 + ((int) (value % (long) settings.DataSize) + 4));
        else
          throw new ArgumentOutOfRangeException("Position value is too large: " + value + ", size: " + this.size);
      }
    }

    public override void Flush()
    {
      this.cf.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.SetPosition();
      int count1 = settings.DataSize - (int) (this.Position % (long) settings.DataSize);
      this.SetPosition();
      if (this.Position + (long) count1 > this.size)
      {
        long position = this.cf.Position;
        return this.cf.Read(buffer, 0, (int) (this.size - this.Position));
      }
      int offset1 = this.cf.Read(buffer, 0, count1);
      for (int index = count - settings.DataSize; offset1 <= index; offset1 += this.cf.Read(buffer, offset1, settings.DataSize))
      {
        this.SetPosition();
        if (this.Position + (long) settings.DataSize >= this.size)
          return offset1 + this.cf.Read(buffer, offset1, (int) (this.size - this.Position));
      }
      if (this.Position + (long) count - (long) offset1 >= this.size)
        return offset1 + this.cf.Read(buffer, offset1, (int) (this.size - this.Position));
      this.SetPosition();
      return offset1 + this.cf.Read(buffer, offset1, count - offset1);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          this.Position = offset;
          break;
        case SeekOrigin.End:
          this.Position = this.size + offset;
          break;
      }
      return this.cf.Seek(offset, SeekOrigin.Current);
    }

    public override void SetLength(long value)
    {
      this.cf.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.cf.Write(buffer, offset, count);
    }

    private void SetPosition()
    {
      this.Position = this.Position;
    }

    public override void Close()
    {
      this.cf.Close();
      base.Close();
    }
  }
}
