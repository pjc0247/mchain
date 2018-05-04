using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class Copyright
    {
        private static readonly string Logo = @"
                  _       _      _           _       
                 (_)     (_)    | |         (_)      
        _ __ ___  _ _ __  _  ___| |__   __ _ _ _ __  
       | '_ ` _ \| | '_ \| |/ __| '_ \ / _` | | '_ \ 
       | | | | | | | | | | | (__| | | | (_| | | | | |
       |_| |_| |_|_|_| |_|_|\___|_| |_|\__,_|_|_| |_|
      
                Minimal implementation of blockchain
                                   Written in CSharp
                                   pjc0247@naver.com
      
";

        public static void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Logo);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
