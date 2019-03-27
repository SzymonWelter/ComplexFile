// Decompiled with JetBrains decompiler
// Type: ComplexStorage.IBlock
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

namespace ComplexStorage
{
  public interface IBlock
  {
    int Type { get; set; }

    int Next { get; set; }

    byte[] ToBytes();
  }
}
