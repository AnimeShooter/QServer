using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;

namespace Qserver.GameServer.Database
{
    public class DatabaseManager
    {
        public static MySqlObjectFactory MySqlFactory = new MySqlObjectFactory("Server=ferib.be;Database=feribra182_QPang;User=feribra182_QPang;Password=Av1a9bBh!22$#1");
        public DatabaseManager()
        {

        }
    }
}
