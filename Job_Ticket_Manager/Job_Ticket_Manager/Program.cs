/// <summary>
///  Program.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace Job_Ticket_Manager
{
    using JobTicketEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialization step

            // Begin running the main window
            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());

        }
    }
}