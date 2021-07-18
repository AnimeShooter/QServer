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
        public Webhost()
        {
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
                // bridge, bunker, castaway, castle, castsle_1, castle_2, castle_3, city, diorama, dollhouse, flycaste, garden, moon, ossyria, sweety, temple
                string name = x.name;
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

            // #====================#
            // #      Player        #
            // #====================#

            // NOTE: Only online
            Get("/player/{id}", async x =>
            {
                uint playerId = x.id;
                var player = Game.Instance.GetPlayer(playerId);
                if (player == null)
                    return new Response().StatusCode = HttpStatusCode.NotFound;

                return Response.AsJson<PlayerAPI>(player.ToAPI());
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

        }
    }
}
