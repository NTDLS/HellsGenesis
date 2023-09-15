using HG.Utility;
using System;
using System.Windows.Forms;

namespace HG
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            HgDevelopmentTools.StripWavFiles(@"C:\NTDLS\HellsGenesis\Hells Genesis\Assets\Sounds");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var form = new FormStartup())
            {
                if (form.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
            }

            Application.Run(new FormMain());
        }
    }
}
