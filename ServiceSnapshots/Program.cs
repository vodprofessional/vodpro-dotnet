using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VUI.VUI2.classes;

namespace ServiceSnapshots
{
    class Program
    {
        public static void Main(string[] args)
        {
            VUIDataFunctions.ExecQuery(@"exec vui_CreateAllServiceMasterSnapshots");
        }
    }
}
