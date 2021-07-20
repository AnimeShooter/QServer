using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang;
using Nancy;
using Newtonsoft.Json;
using Qserver.GameServer.Database.Repositories;

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
                byte[] body = new byte[Request.Body.Length];
                Request.Body.Read(body, 0, body.Length);

                dynamic registerRequest = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body));

                string ip = Request.Headers["X-Forwarded-For"].ToString(); // proxy IP
                string username = registerRequest["Username"];
                string email = registerRequest["Email"];
                string password = registerRequest["Password"];

                if(username == "" || email == "")
                {
                    return Response.AsJson(new APIResponse<string>()
                    {
                        Message = "Error, invalid username or email."
                    });
                }
                else if(password.Length < 6)
                {
                    return Response.AsJson(new APIResponse<string>()
                    {
                        Message = "Error, password must be atleast 6 characters long."
                    });
                }

                // check existing
                List<DBUser> existingUsers = Game.Instance.UsersRepository.UserExists(username, email).Result;
                if(existingUsers.Count > 0)
                {
                    return Response.AsJson(new APIResponse<string>()
                    {
                        Message = "Error, email or username already in use."
                    });
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                uint userId = Game.Instance.UsersRepository.CreateUser(username, email, hashedPassword, ip).Result;
                if(userId == 0)
                {
                    return Response.AsJson(new APIResponse<string>()
                    {
                        Message = "Error, failed to create user."
                    });
                }

                uint playerId = Game.Instance.PlayersRepository.CreatePlayer(userId, username, 2500, 100).Result;
                if (playerId == 0)
                {
                    return Response.AsJson(new APIResponse<string>()
                    {
                        Message = "Critical Error, failed to create player!"
                    });
                }

                Game.Instance.PlayersRepository.CreatePlayerStats(playerId).GetAwaiter().GetResult();
                foreach (var cid in EquipmentManager.CharacterIds)
                    Game.Instance.PlayersRepository.CreatePlayerEquipments(playerId, cid).GetAwaiter().GetResult();

                Player you = new Player(playerId);
                return Response.AsJson(new APIResponse<PlayerAPI>(){
                    Result = you.ToAPI()
                });
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
