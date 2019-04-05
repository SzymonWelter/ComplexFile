using System;
using System.Linq;
using System.Text;
using ComplexStorage;

public static class Utilities
{
    public static byte[] ToBytes(ICFSettings settings)
    {
        throw new NotImplementedException();
    }

    public static string GenerateId()
    {
        StringBuilder id = new StringBuilder();
        Enumerable.Range(65, 26).Select((e => ((char)e).ToString()))
            .Concat(Enumerable.Range(97, 26).Select((e => ((char)e).ToString())))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString())).OrderBy(e => Guid.NewGuid())
            .Take(ComplexFile.Settings.IdSize - 1).ToList().ForEach(e => id.Append(e));
        return id.ToString();
    }
}