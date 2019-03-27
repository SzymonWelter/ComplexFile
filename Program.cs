// Decompiled with JetBrains decompiler
// Type: ComplexStorage.Program
// Assembly: Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9E5EB5E8-B477-4126-A7A9-28C197CDD219
// Assembly location: E:\user\Documents\webdav\WebDav\bin\Debug\Test.exe

using System;
using System.Linq;

namespace ComplexStorage
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      using (ComplexFile cf = ComplexFile.OpenOrCreate("E:\\user\\Documents\\webdav\\WebDav\\bin\\Debug\\ComplexFiles\\new.dmsw"))
      {
        byte[] buffer = new byte[504];
        using (ProxyStream proxyStream = new ProxyStream(cf, cf.GetDirectoryById("0UzDAqxrw54E8tf0IX").First<FileHeader>()))
          proxyStream.Read(buffer, 0, 504);
      }
      Console.WriteLine();
      Console.ReadKey();
    }
  }
}
