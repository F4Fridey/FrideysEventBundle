using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace FrideysEventBundle
{
    class DeathmatchEventHandler : IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerPlayerDie, IEventHandlerFixedUpdate
    {
        private readonly FrideysEventBundle plugin;

        public DeathmatchEventHandler(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            try
            {
                if (plugin.eventQeue[0] == "deathmatch")
                {
                    if (!BeginEvent())
                    {
                        plugin.Debug("Not enough players for event.");
                    }
                }
            }
            catch
            {

            }
        }

        Player[] players;
        int[] stats;

        private bool eventRunning = false;
        float time;
        float inbetweenTime;
        int mode;/*0=on Surface with less than 11 ppl|1= in facility with 11 or more ppl*/

        List<Smod2.API.Door> doorsList = new List<Smod2.API.Door>();

        public bool BeginEvent()
        {
            players = new List<Player>().ToArray();
            players = GetPlayers().ToArray();
            if (players.Length >= 4 && players.Length <= 10)
            {
                mode = 0;
                List<int> beginStats = new List<int>();
                for (int i = 0; i < players.Length; i++)
                {
                    beginStats.Add(0);
                }
                stats = beginStats.ToArray();
                plugin.Round.RoundLock = true;
                for (int i = 0; i < players.Length; i++)
                {
                    plugin.Info(players[i].Name);
                    plugin.Info(stats[i].ToString());
                }
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    try { lift.Locked = true; }
                    catch { plugin.Debug(lift.ToString() + " is not lockable."); }
                }
                List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
                foreach (Smod2.API.Door door in doors)
                {
                    if (door.Name == "SURFACE_GATE" || door.Name == "NUKE_SURFACE")
                    {
                        door.Open = true;
                        door.Locked = true;
                    }else if (door.Name == "ESCAPE_INNER")
                    {
                        door.Open = false;
                        door.Locked = true;
                    }
                }
                foreach (Player player in players)
                {
                    try
                    {
                        player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
                        player.Teleport(new Vector(10.6f, 989, -48.6f));
                        player.SetGodmode(true);
                        extraText = "<color=#00ff00>Deathmatch! Get the most kills, Invincibility latsts for 30 seconds!</color>";
                        player.GiveItem(Smod2.API.ItemType.USP);
                        player.GiveItem(Smod2.API.ItemType.P90);
                        player.GiveItem(Smod2.API.ItemType.MP7);
                        player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                        player.GiveItem(Smod2.API.ItemType.COM15);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                        player.GiveItem(Smod2.API.ItemType.MEDKIT);
                    }
                    catch { }
                }
                time = 450;
                inbetweenTime = 30;
                eventRunning = true;
                return true;
            }
            else if (players.Length > 10)
            {
                mode = 1;
                List<int> beginStats = new List<int>();
                for (int i = 0; i < players.Length; i++)
                {
                    beginStats.Add(0);
                }
                stats = beginStats.ToArray();
                plugin.Round.RoundLock = true;
                for (int i = 0; i < players.Length; i++)
                {
                    plugin.Info(players[i].Name);
                    plugin.Info(stats[i].ToString());
                }
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    try { lift.Locked = true; }
                    catch { plugin.Debug(lift.ToString() + " is not lockable."); }
                }
                List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
                doorsList.Clear();
                Vector ezCheckpoint = new Vector(0, 0, 0);
                foreach (Smod2.API.Door door in doors)
                {
                    if (door.Name == "096" || door.Name == "HCZ_ARMORY" || door.Name == "106_PRIMARY" || door.Name == "106_SECONDARY" || door.Name == "106_BOTTOM")
                    {
                        door.Locked = true;
                    }
                    if (door.Position.y >= -900 && door.Position.y <= -1100 && door.Name != "CHECKPOINT_EZ")
                    {
                        doorsList.Add(door);
                        door.Open = true;
                    }
                }
                foreach (Player player in players)
                {
                    try
                    {
                        player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
                        System.Random rnd = new System.Random();
                        int pos = rnd.Next(0, doorsList.Count);
                        player.Teleport(doorsList[pos].Position);
                        player.SetGodmode(true);
                        extraText = "<color=#00ff00>Deathmatch! Get the most kills, Invincibility latsts for 30 seconds!</color>";
                        player.GiveItem(Smod2.API.ItemType.USP);
                        player.GiveItem(Smod2.API.ItemType.P90);
                        player.GiveItem(Smod2.API.ItemType.MP7);
                        player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                        player.GiveItem(Smod2.API.ItemType.COM15);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                        player.GiveItem(Smod2.API.ItemType.MEDKIT);
                    }
                    catch { }
                }
                time = 450;
                inbetweenTime = 30;
                eventRunning = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (eventRunning)
            {
                float timeLeft;
                ev.Player.OverwatchMode = true;
                timeLeft = (time - timer) / 60;
                ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating. Event will end in " + timeLeft.ToString("0.0") + " minutes.", false);
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (eventRunning)
            {
                EndEvent();
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            if (eventRunning)
            {
                EndEvent();
            }
        }

        void EndEvent()
        {
            foreach (Player player in GetPlayers())
            {
                player.OverwatchMode = false;
            }
            timer = 0;
            eventRunning = false;
            inbetweenTimerActivated = false;
            inbetweenTimerActivated2 = false;
            inbetweenTimer = 0;
            gameOver = false;
            extraText = "";
        }

        public List<Player> GetPlayers()
        {
            return plugin.Server.GetPlayers();
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (eventRunning)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (ev.Killer.UserId == players[i].UserId && ev.Killer.UserId != ev.Player.UserId)
                    {
                        stats[i] += 1;
                    }
                }
                ev.SpawnRagdoll = false;
            }
        }

        float timer=0;
        float inbetweenTimer=0;
        float secondTimer =0;

        bool inbetweenTimerActivated = false;
        bool inbetweenTimerActivated2 = false;
        bool gameOver = false;

        string extraText = "";

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (eventRunning)
            {
                if (timer < time && !gameOver)
                {
                    timer += Time.deltaTime;
                    inbetweenTimer += Time.deltaTime;
                }
                else if (!gameOver)
                {
                    gameOver = true;
                    int topStat = 0;
                    List<Player> topPlayers = new List<Player>();
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (stats[i] > topStat)
                        {
                            topStat = stats[i];
                            try { topPlayers.RemoveAt(0); } catch { }
                            topPlayers.Add(players[i]);
                        }
                    }
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (stats[i] == topStat && players[i] != topPlayers[0])
                        {
                            topPlayers.Add(players[i]);
                        }
                    }
                    foreach (Player player in players)
                    {
                        try
                        {
                            player.ChangeRole(Smod2.API.RoleType.TUTORIAL, true, false);
                            string str = "<color=#07d100>Winner: ";
                            foreach (Player topPlayer in topPlayers)
                            {
                                str += topPlayer.Name + " ";
                            }
                            str += "with " + topStat + " kill(s)!</color>";
                            player.PersonalBroadcast(10, str, false);
                        }catch { }
                    }
                    plugin.Round.RoundLock = false;
                }
                if (inbetweenTimer > inbetweenTime && !inbetweenTimerActivated)
                {
                    foreach (Player player in players)
                    {
                        try
                        {
                            player.SetGodmode(false);
                        }
                        catch { }
                    }
                    extraText = "<color=#ff0000>Invincibility Over. The one with the most kills in 7 minutes wins!</color>";
                    inbetweenTimerActivated = true;
                    inbetweenTime = 10;
                    inbetweenTimer = 0;
                }
                else if (inbetweenTimer > inbetweenTime && !inbetweenTimerActivated2)
                {
                    extraText = "";
                    inbetweenTimerActivated2 = true;
                    inbetweenTime = 60;
                    inbetweenTimer = 0;
                }
                else if (inbetweenTimer > inbetweenTime)
                {
                    for (int i = 0; i <= 35; i++)
                        foreach (Smod2.API.Item item in plugin.Server.Map.GetItems((Smod2.API.ItemType)i, true))
                            item.Remove();
                    inbetweenTime = 60;
                    inbetweenTimer = 0;
                }
                if (secondTimer < 1)
                {
                    secondTimer += Time.deltaTime;
                }
                else if (!gameOver)
                {

                    secondTimer = 0;
                    int topStat = 0;
                    Player topPlayer = null;
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (stats[i] > topStat)
                        {
                            topStat = stats[i];
                            topPlayer = players[i];
                        }
                    }
                    for (int i = 0; i < players.Length; i++)
                    {
                        try
                        {
                            string str = "";
                            if (topStat == 0)
                            {
                                str = "<color=#fcba03>1st: No One </color> | <color=#07d100>Your Kills: " + stats[i] + "</color> | Time left: " + (time - timer).ToString("0") + "\n" + extraText;
                            }
                            else
                            {
                                str = "<color=#fcba03>1st: " + topStat + " " + topPlayer.Name + "</color> | <color=#07d100>Your Kills: " + stats[i] + "</color> | Time left: " + (time - timer).ToString("0") + "\n" + extraText;
                            }
                            players[i].PersonalBroadcast(1, str, false);
                        }
                        catch { }
                    }
                    try
                    {
                        plugin.Info("1");
                        List<Player> newPlayerList = GetPlayers();
                        foreach (Player player in newPlayerList)
                        {
                            if (player.TeamRole.Role == Smod2.API.RoleType.SPECTATOR && player.OverwatchMode == false)
                            {
                                plugin.Info("2");
                                player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
                                plugin.Info("3");
                                RandomTP(player, mode);
                                plugin.Info("4");
                                player.GiveItem(Smod2.API.ItemType.USP);
                                player.GiveItem(Smod2.API.ItemType.P90);
                                player.GiveItem(Smod2.API.ItemType.MP7);
                                player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                                player.GiveItem(Smod2.API.ItemType.COM15);
                                plugin.Info("5");
                                player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                                player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                                player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                                plugin.Info("6");
                                player.GiveItem(Smod2.API.ItemType.MEDKIT);
                            }
                        }
                        plugin.Info("7");
                    }
                    catch { plugin.Info("=== Error 1"); }
                }
            }
        }

        void RandomTP(Player player, int mode/*0=on Surface with less than 11 ppl|1= in facility with 11 or more ppl*/)
        {
            System.Random rnd = new System.Random();
            if (mode == 0)
            {
                int pos = rnd.Next(0, 13);
                switch (pos)
                {
                    default:
                        player.Teleport(new Vector(40.1f, 989, -36.9f));
                        break;
                    case 0:
                        player.Teleport(new Vector(10.9f, 989, -48.4f));
                        break;
                    case 1:
                        player.Teleport(new Vector(-50.6f, 989, -58.7f));
                        break;
                    case 2:
                        player.Teleport(new Vector(14.6f, 997, -46.1f));
                        break;
                    case 3:
                        player.Teleport(new Vector(14.4f, 997, -20.5f));
                        break;
                    case 4:
                        player.Teleport(new Vector(-10.4f, 1002, -19.9f));
                        break;
                    case 5:
                        player.Teleport(new Vector(0.1f, 1002, -55.9f));
                        break;
                    case 6:
                        player.Teleport(new Vector(73.7f, 989, -48.7f));
                        break;
                    case 7:
                        player.Teleport(new Vector(86.6f, 989, -69.4f));
                        break;
                    case 8:
                        player.Teleport(new Vector(150.2f, 995, -70.5f));
                        break;
                    case 9:
                        player.Teleport(new Vector(149.6f, 995, -45.9f));
                        break;
                    case 10:
                        player.Teleport(new Vector(187.4f, 994, -79.6f));
                        break;
                    case 11:
                        player.Teleport(new Vector(187.5f, 994, -30.4f));
                        break;
                    case 12:
                        player.Teleport(new Vector(40.1f, 989, -36.9f));
                        break;
                }
            }else if (mode == 1)
            {
                int pos = rnd.Next(0, doorsList.Count);
                player.Teleport(doorsList[pos].Position);
            }
        }
    }
}
