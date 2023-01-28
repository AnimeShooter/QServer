using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using System.IO;

namespace Qserver.Database
{
    public class DatabaseManager
    {

        public static MySqlObjectFactory MySqlFactory = new MySqlObjectFactory(File.ReadAllText("connstring.txt"));
        public DatabaseManager()
        {

        }
    }
}
