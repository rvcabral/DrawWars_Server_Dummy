using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.GameManager
{
    public static class Themes
    {

        private static List<string> _Themes;

        public static List<string> ThemesList {
            get {
                if (_Themes == null)
                    _Themes = new List<string>(System.IO.File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "Data\\Themes.txt")));
                
                return _Themes;
            }
        }
    }
}
