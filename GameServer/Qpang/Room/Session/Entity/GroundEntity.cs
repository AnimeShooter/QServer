﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    struct GroundEntity
    {
        public uint entityId;
        public TimeSpan DestroyalTime;
        public float X;
        public float Y;
        public float Z;
    }
}