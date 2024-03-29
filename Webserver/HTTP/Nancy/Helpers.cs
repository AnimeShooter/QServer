﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qserver.Database.Repositories;
using Nancy;
using Qserver.GameServer.Qpang;
using Qserver.GameServer;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Qserver.Webserver.HTTP.Nancy
{
    public static class Helpers
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

        public static bool IsValidReCaptcha(string token)
        {
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create($"https://www.google.com/recaptcha/api/siteverify?secret={Settings.ReCaptchaSecret}&response={token}");
            webreq.Method = "POST";
            WebResponse webres = webreq.GetResponse();
            Stream receiveStream = webres.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            dynamic reCaptcheResponse = JsonConvert.DeserializeObject(readStream.ReadToEnd());
            if (reCaptcheResponse["success"] == null || reCaptcheResponse["success"] == false)
                return false;
            return true;
        }
    }
}
