using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qserver.Util;

namespace Qserver.GameServer.Database
{
    // NOTE:
    //Database:	feribra182_QPang
    //Host:	localhost
    //Username:	feribra182_QPang
    //Password:	Av1a9bBh!22$#1

    public class Memory
    {
        public static int Count;
        public static ConcurrentDictionary<int, object> TestData;

        public static void Init()
        {
            Log.Message(LogType.NORMAL, "Loading Database Memory...");

            //TestData = DBReader.Read<uint, object>("Test", "Id");
            //Log.Message(LogType.NORMAL, $"Loaded {Count} DBCs.");
        }
    }
}
