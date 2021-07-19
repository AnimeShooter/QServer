using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang;
using Nancy;


namespace Qserver.External.HTTP.Nancy
{
    public class Webhost : NancyModule
    {
        private static object _accountCreation = new object(); // lock for acc creation?

        public Webhost()
        {
            
            // #====================#
            // #       User         #
            // #====================#

            Post("/user/register/", async x =>
            {
                // DB Create:
                // users (login, pw, ..)
                // players (qpang player)
                // player_equipment (6x total characters, blank inserts)
                // player_stats (1x insert blank)
                // TODO: add captcha? cuz its intensive hehe
                return null;
            });

            Post("/user/login/", async x =>
            {
                // TODO: create authorization token using login stuff
                return null;
            });

            Post("/user/update/", async x =>
            {
                // TODO: allow user to update password (and mock qpangIO with it?)
                return null;
            });

            Get("/user/info/", async x =>
            {
                // Get user info
                return null;
            });

            // #====================#
            // #      Player        #
            // #====================#

            Get("/player/online", async x =>
            {
                List<PlayerAPI> APIPlayers = new List<PlayerAPI>();
                var players = Game.Instance.PlayersList();
                foreach (var p in players)
                    APIPlayers.Add(p.ToAPI());
                return Response.AsJson<List<PlayerAPI>>(APIPlayers);
            });

            // TODO: Only online, fix?
            Get("/player/{id}", async x =>
            {
                uint playerId = x.id;
                var player = Game.Instance.GetPlayer(playerId);
                if (player == null)
                    return new Response().StatusCode = HttpStatusCode.NotFound;

                return Response.AsJson<PlayerAPI>(player.ToAPI());
            });

            // TODO
            Get("/player/leaderboard", async x =>
            {
                // TODO
                return null;
            });

            // #====================#
            // #       Rooms        #
            // #====================#

            Get("/room/", async x =>
            {
                List<RoomAPI> roomApis = new List<RoomAPI>();
                var rooms = Game.Instance.RoomManager.List();
                foreach (var room in rooms)
                    roomApis.Add(room.ToAPI());

                return Response.AsJson<List<RoomAPI>>(roomApis);
            });

            Get("/room/{id}", async x =>
            {
                uint roomId = x.id;
                var room = Game.Instance.RoomManager.Get(roomId);
                if (room == null)
                    return new Response().StatusCode = HttpStatusCode.NotFound;

                return Response.AsJson<RoomAPI>(room.ToAPI());
            });

            // #====================#
            // #        Misc        #
            // #====================#

            Get("/img/level/{lvl}", async x =>
            {
                string lvl = x.lvl;
                string filename = Directory.GetCurrentDirectory() + $"/External/HTTP/Public/img/levels/{lvl}.png";
                if (!File.Exists(filename))
                    return new Response().StatusCode = HttpStatusCode.NotFound;

                var response = new Response()
                {
                    ContentType = "image/png",
                    Contents = stream =>
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(File.ReadAllBytes(filename));
                        }
                    }
                };
                return response;
            });

            Get("/img/maps/small/{name}", async x =>
            {
                //string[] mapNames = new string[] { "garden", "diorama", "fly", "keep", "doll", "sweety", "river", "bunker", "temple", "bridge", "castaway" };
                string[] mapNames = new string[] {"", "diorama", "fly", "keep", "doll", "garden", "river", "practice", "bunker", "temple", "fly", "bridge", "castaway", "garden" }; // Overflow??
                int mapId = -1;
                string name = x.name;
                if (Int32.TryParse(x.name, out mapId))
                {
                    if (mapId >= 0 && mapId < mapNames.Length)
                        name = mapNames[mapId];
                }
 
                string filename = Directory.GetCurrentDirectory() + $"/External/HTTP/Public/img/maps/small/{name}.png";
                if (!File.Exists(filename))
                    return new Response().StatusCode = HttpStatusCode.NotFound;

                var response = new Response()
                {
                    ContentType = "image/png",
                    Contents = stream =>
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(File.ReadAllBytes(filename));
                        }
                    }
                };
                return response;
            });

        }
    }
}
