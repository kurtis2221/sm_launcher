using System;
using System.IO;
using System.Windows.Forms;

namespace sm_launcher
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            if (!File.Exists(GlobalHandler.CFG_FILE))
            {
                GlobalHandler.ErrorMsg(GlobalHandler.CACHE_FILE +
                    " file not found!\nCreate a configuration before running!");
                return;
            }
            Application.Run(new Launcher());
        }
    }
}