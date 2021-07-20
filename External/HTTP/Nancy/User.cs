using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qserver.GameServer.Database.Repositories;
using Nancy;
using Qserver.GameServer.Qpang;

namespace Qserver.External.HTTP.Nancy
{
    public static class User
    {
        public static DBUser? UserAuth(Request Request)
        {
            if (Request.Headers.Authorization == "")
                return null;

            var user = Game.Instance.UsersRepository.GetUser(Request.Headers.Authorization).Result;
            if (user.Count == 0 || user.Count > 1)
                return null;
               

            return user[0];
        }
    }
}
