using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessTinKin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ChessUI ui = new ChessUI();
            ChessCode code = new ChessCode();

            Thread uiThread = new Thread(() => {
                ui.ShowDialog();
            });

            Thread codeThread = new Thread(() => {
                code.Run(ui);
            });

            uiThread.Start();
            codeThread.Start();

        }
    }
}
