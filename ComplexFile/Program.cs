using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            using(ComplexFile cf = ComplexFile.OpenOrCreate("twoja_stara"))
            {
                cf.CreateDirectory("zapierdala2", "root");
            }

        }
    }
}
