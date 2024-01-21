using Si.GameEngine.Core.Managers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Si.Game
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0 || args.Where(o => o.ToLower() == "/nosplash").Any() == false)
            {
                using (var form = new FormStartup())
                {
                    if (form.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }

            Application.Run(new FormRenderTarget());
        }
    }
}
