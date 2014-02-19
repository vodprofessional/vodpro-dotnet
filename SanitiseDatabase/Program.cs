using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VUI.Sanitiser;

namespace SanitiseDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            DBSanitiser.Sanitise();
        }
    }
}
