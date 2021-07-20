using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang;
using Nancy;
using Newtonsoft.Json;
using Qserver.GameServer.Database.Repositories;
using Qserver.GameServer;
using Nancy.TinyIoc;
using Nancy.Conventions;
using Nancy.Bootstrapper;

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
                string reToken = registerRequest["reToken"];

                if (username == "" || email == "")
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, invalid username or email." });
                
                if (password.Length < 6)
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, password must be atleast 6 characters long." });

#if !DEBUG
                if (teToken == "" || !Helpers.IsValidReCaptcha(reToken)) // robot check
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, you might be a robot." });
#endif

                // check existing
                List<DBUser> existingUsers = Game.Instance.UsersRepository.UserExists(username, email).Result;
                if (existingUsers.Count > 0)
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, email or username already in use." });

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                string token = Util.Util.GenerateToken();
                uint userId = Game.Instance.UsersRepository.CreateUser(username, email, hashedPassword, ip, token).Result;
                if (userId == 0)
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, failed to create user." });

                uint playerId = Game.Instance.PlayersRepository.CreatePlayer(userId, username, 7500, 25).Result;
                if (playerId == 0)
                    return Response.AsJson(new APIResponse<string>() { Message = "Critical Error, failed to create player!" });

                Game.Instance.PlayersRepository.CreatePlayerStats(playerId).GetAwaiter().GetResult();
                foreach (var cid in EquipmentManager.CharacterIds)
                    Game.Instance.PlayersRepository.CreatePlayerEquipments(playerId, cid).GetAwaiter().GetResult();

                Player you = new Player(playerId);

                uint[] startWeapons = new uint[] { 1095172097, 1095303169, 1095368704, 1095434241 };
                foreach (var itemid in startWeapons)
                {
                    var card = new InventoryCard()
                    {
                        IsActive = false,
                        IsOpened = true,
                        IsGiftable = false,
                        ItemId = itemid,
                        PeriodeType = 254,
                        Period = 1,
                        PlayerOwnedId = you.PlayerId,
                        Type = 87
                    };
                    Game.Instance.ItemsRepository.PurchaseItem(card, you).GetAwaiter().GetResult();
                }   

                var res = Response.AsJson(new APIResponse<PlayerAPI>()
                {
                    Result = you.ToAPI()
                });
                res.Headers.Add("x-auth-token", token);
                return res;
            });

            Post("/user/login/", async x =>
            {
                // read body
                byte[] body = new byte[Request.Body.Length];
                Request.Body.Read(body, 0, body.Length);

                dynamic registerRequest = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body));
                string username = registerRequest["Username"];
                string password = registerRequest["Password"];
                string reToken = registerRequest["reToken"];

                if (username == "" || password == "")
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, invalid Login" });

#if !DEBUG
                if (teToken == "" || !Helpers.IsValidReCaptcha(reToken)) // robot check
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, you might be a robot." });
#endif

                var user = Game.Instance.UsersRepository.GetUserCredentials(username).Result;
                if (user.password == null || !BCrypt.Net.BCrypt.Verify(password, user.password))
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, invalid Login" });

                var id = Game.Instance.UsersRepository.GetPlayerId(user.id).Result;
                Player player = new Player(id); // TODO: improve?

                user.token = Util.Util.GenerateToken();
                Game.Instance.UsersRepository.UpdateToken(user.id, user.token).GetAwaiter().GetResult();

                var res = Response.AsJson(new APIResponse<PlayerAPI>()
                {
                    Result = player.ToAPI()
                });
                res.Headers.Add("x-auth-token", user.token);
                return res;
            });

            Post("/user/update/", async x =>
            {
                var user = Helpers.UserAuth(Request);
                if (user == null)
                    return Response.AsJson(new APIResponse<string>() { Message = "Authentication error." });

                // read body
                byte[] body = new byte[Request.Body.Length];
                Request.Body.Read(body, 0, body.Length);

                dynamic registerRequest = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body));
                string newPassword = registerRequest["NewPassword"];
                if (newPassword.Length < 6)
                    return Response.AsJson(new APIResponse<string>() { Message = "Error, password must be atleast 6 characters long." });

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                Game.Instance.UsersRepository.UpdatePassword(user.Value.id, hashedPassword).GetAwaiter().GetResult();

                // NOTE: update session token? or its QPang 2013 all over again xD

                return Response.AsJson(new APIResponse<string>() { Message = "Your password has been reset!" });
            });

            Get("/user/info/", async x =>
            {
                var user = Helpers.UserAuth(Request);
                if (user == null)
                    return Response.AsJson(new APIResponse<string>() { Message = "Authentication error." });

                var info = Game.Instance.PlayersRepository.GetPlayerByUserId(user.Value.id).Result;
                Player player = new Player(info.id); // TODO: improve?

                return Response.AsJson(new APIResponse<PlayerAPI>() { Result = player.ToAPI() });
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
                string[] mapNames = new string[] { "", "diorama", "fly", "keep", "doll", "garden", "river", "practice", "bunker", "temple", "fly", "bridge", "castaway", "garden" }; // Overflow??
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
