# 🔫 Qpang Server

QServer is a private server project for the Qpang aka MangaFighter game. The project was called 'Project AnimeShooter' and is currently being hosted on [AnimeShooter.com](https://animeshooter.com), however, due to the lack of players _(literally f*cking zero)_ the project has been made open-source and is meant to be used as a learning resource.

## 🎓 Server History

The QServer was developed to recreate the amazing 2013 experience I once had as a 12y/old kid, unfortunately, the nostalgic memories have fooled me as there is nothing 'amazing' about this, especially once I saw how poorly the game was designed. Anyway, this pserver was designed to create an identical experience which means no new or existing item mods, bug fixes, or other crap. The infamous 'reload glitch' - that is, cancel reload animation to faster reload - will stay in the game and is meant to be (ab)used during gameplay.

# 📃 Features
- [x] PvP _(DM, TDM, Essence, VIP, TagPlay*, Practice)_
- [x] Friend list
- [x] Gifting
- [x] Chat
- [x] Channels
- [x] Web API
- [x] Coupon Codes
- [x] Crane
- [x] Skill cards* _(not all)_
- [ ] Lootbox _(Panthalassa Chest)_
- [ ] Achievements*
- [ ] Memos*
- [ ] PvE* _(StO)_
- [ ] Trading*
- [ ] Boosting*

_*Work in progress_

## 📐 Setup
Learning about private servers is best done with hands-on stuff, therefore I think it's important for anyone to quickly set up a server. The server was made with open-source in mind as _(almost)_ every technology used is open-source/freely available.

Requirements:
 - `.net 5.0`
 - `MySql`

Setup:
1. Create a MySql database
2. Run all `sql_dump` exports in your new database
3. `git clone https://github.com/AnimeShooter/QServer.git`
4. `cd QServer`
5. `vim Database/DatabaseManager.cs` *(change `connstring` on line 10)*
6. `dotnet run -c Release`

NOTE: step 3 should be done inside a config file, but can't be arsed.

TODO: move `connstring` to config

Server Args:
- `--NoAuth` Disable Auth Server _(port 8003, initial `IP=` connection)_
- `--NoLobby` Disable Lobby Server _(port 8005, lobby/walking area before entering room)_
- `--NoSquare` Disable Square Server _(port 8012, channel after auth)_
- `--WebSocket` Enable WebSocket, used for live website stats _(port 8026)_
- `--WebAPI` Enable WebAPI, used for website/fansite _(port 8088/80)_
- `--Debug` Enable debug prints
- `--CLI` Enable server CLI for random commands

## 🎮 How to play
First, you need to install the original game installed from the AnimeShooter mirror. Once the game is installed you can't run it as normal, instead, you have to patch out the anti-cheating and modify the `QPangID.ini` to replace the host IP with our local server _(probably `localhost` in your case)_.

To do so, you will need to use the [patched mini launcher](https://github.com/AnimeShooter/QPangMiniLauncher) to start the game.

Instructions:
1. Download & install the [official game installer](https://animeshooter.com/download/QPangSetup.exe)
2. Go to the installation folder, default path is `C:\Program Files (x86)\Realfogs\QPang`
3. Open `QPangID.ini` and change line 2 to `IP=localhost`
4. Download & install the [patched mini launcher](https://github.com/AnimeShooter/QPangMiniLauncher)
5. Move the `QPangPatcher.exe` into the `QPang` folder
6. Drag & drop `QPangBin.exe` onto `QpangPatcher.exe`
7. Launch `QPangBin.exe -fullscreen:0 -width:800 -height:600 -forcevsync:0 -locale:English` _(or write that into `start.bat` for ease of use)_

Enjoy, to restart the game you will only have to launch the `QPangBin.exe` with the required command line _(or use the `start.bat`)_.

Note: You might want to set `IP=AnimeShooter.com` to connect to our remote servers instead ;D!

# 🙈 Credits

- Dennis & [Deluze](https://github.com/Deluze) for developing the [qpang-essence-emulator](https://github.com/ferib/qpang-essence-emulator) that I forked years ago.
- [Ferib](https://ferib.dev/), (me), for re-writing the [qpang-essence-emulator](https://github.com/ferib/qpang-essence-emulator) to C# and removing the Windows specific components as well as extending the features of the private server
- [Blumster](https://github.com/Blumster) this hero for making [TNL.NET](https://github.com/Blumster/TNL.NET) and assisting me with the implementation, it is used in the game rooms.

# ♥ Support
Running into issues while setting things up? found a bug? or just enjoying our work? Please consider [♥ donating](https://github.com/sponsors/ferib) to get help _(faster?)_