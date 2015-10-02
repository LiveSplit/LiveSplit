using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UpdateManager
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                /*args = new String[4];
                args[0] = "http://irc107.xe.cx/update.xml";
                args[1] = "http://irc107.xe.cx/update/";
                args[2] = "0.2.0.0";
                args[3] = "D:\\Projekte\\C#\\IRC107\\bin\\Release\\IRC107.exe";*/
                //"http://irc107.xe.cx/update.xml" "http://irc107.xe.cx/update/" 0.2.0.0 "D:\Projekte\C#\IRC107\bin\Release\IRC107.exe"
                if (args.Length >= 3)
                {
                    List<IUpdateable> updateables = new List<IUpdateable>();
                    for (int i=0; i+2 < args.Length; i += 3)
                        updateables.Add(new Updateable(args[i], args[i+1], Version.Parse(args[i+2])));

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new UpdateForm(updateables, (args.Length % 3 == 1) ? args[args.Length - 1] : null));
                }
            }
            catch (Exception) { }
        }
    }
}
