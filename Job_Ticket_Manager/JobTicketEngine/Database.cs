/// <summary>
///  Database.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace JobTicketEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Database
    {
        /// <summary>
        ///  Path to database on computer.
        /// </summary>
        private string databaseName;

        public Database()
        {
            this.databaseName = AppDomain.CurrentDomain.BaseDirectory;
        }
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
    }
}