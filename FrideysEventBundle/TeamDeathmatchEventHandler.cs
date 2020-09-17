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
    class TeamDeathmatchEventHandler : IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerPlayerDie, IEventHandlerFixedUpdate
    {
        private readonly FrideysEventBundle plugin;

        public TeamDeathmatchEventHandler(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            try
            {
                if (plugin.eventQeue[0] == "teamdeathmatch")
                {
                    plugin.Info("1");
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
        List<Player> players = new List<Player>();

        List<Player> ntf = new List<Player>();
        List<Player> chaos = new List<Player>();
        int ntfKills;
        int chaosKills;

        List<Smod2.API.Door> posDoors = new List<Smod2.API.Door>();

        private bool eventRunning = false;
        float time;
        float inbetweenTime;

        public bool BeginEvent()
        {
            players = GetPlayers();
            
            if (players.Count >= 8)
            {
                ntfKills = 0;
                chaosKills = 0;
                plugin.Round.RoundLock = true;
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    try { lift.Locked = true; }
                    catch { plugin.Debug(lift.ToString() + " is not lockable."); }
                }
                plugin.Info("2");
                ntf = new List<Player>();
                chaos = new List<Player>();
                for (int i = 0; i < players.Count; i++)
                {
                    if (i < (players.Count / 2))
                    {
                        ntf.Add(players[i]);
                        plugin.Info("added to ntf " + players[i].Name);
                    }
                    else if (i >= (players.Count / 2) && i < players.Count)
                    {
                        chaos.Add(players[i]);
                        plugin.Info("added to chaos " + players[i].Name);
                    }
                }
                List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
                posDoors = new List<Smod2.API.Door>();
                plugin.Info("3");
                foreach (Smod2.API.Door door in doors)
                {
                    if (door.Position.y < -990 && door.Position.y > -1010 && door.Name != "CHECKPOINT_ENT")
                    {
                        posDoors.Add(door);
                        plugin.Info("added to posDoors " + door.Name);
                    }
                }
                plugin.Info("4");
                System.Random rnd = new System.Random();
                foreach (Player player in players)
                {
                    if (ntf.Contains(player))
                    {
                        player.ChangeRole(Smod2.API.RoleType.NTF_LIEUTENANT, true, false);
                        plugin.Info("changed role to ntf " + player.Name);
                        int pos = rnd.Next(0, posDoors.Count);
                        player.Teleport(new Vector(posDoors[pos].Position.x, posDoors[pos].Position.y + 1, posDoors[pos].Position.z));
                        plugin.Info("tped to room ntf " + player.Name);
                        extraText = "<color=#00ff00>Team Deathmatch! Team with the most kills wins!</color>";
                        foreach (Smod2.API.Item item in player.GetInventory())
                            item.Remove();
                        player.GiveItem(Smod2.API.ItemType.USP);
                        player.GiveItem(Smod2.API.ItemType.P90);
                        player.GiveItem(Smod2.API.ItemType.MP7);
                        player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                        player.GiveItem(Smod2.API.ItemType.COM15);
                        player.GiveItem(Smod2.API.ItemType.O5_LEVEL_KEYCARD);
                        player.GiveItem(Smod2.API.ItemType.WEAPON_MANAGER_TABLET);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                        player.GiveItem(Smod2.API.ItemType.MEDKIT);
                    }else if (chaos.Contains(player))
                    {
                        player.ChangeRole(Smod2.API.RoleType.CHAOS_INSURGENCY, true, false);
                        plugin.Info("changed role to chaos " + player.Name);
                        int pos = rnd.Next(0, posDoors.Count);
                        player.Teleport(new Vector(posDoors[pos].Position.x, posDoors[pos].Position.y + 1, posDoors[pos].Position.z));
                        plugin.Info("tped to room chaos " + player.Name);
                        extraText = "<color=#00ff00>Team Deathmatch! Team with the most kills in 7 minutes wins!</color>";
                        foreach (Smod2.API.Item item in player.GetInventory())
                            item.Remove();
                        player.GiveItem(Smod2.API.ItemType.USP);
                        player.GiveItem(Smod2.API.ItemType.P90);
                        player.GiveItem(Smod2.API.ItemType.MP7);
                        player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                        player.GiveItem(Smod2.API.ItemType.COM15);
                        player.GiveItem(Smod2.API.ItemType.O5_LEVEL_KEYCARD);
                        player.GiveItem(Smod2.API.ItemType.WEAPON_MANAGER_TABLET);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                        player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                        player.GiveItem(Smod2.API.ItemType.MEDKIT);
                    }
                }
                time = 420;
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
            if (eventRunning && ev.Player.UserId != ev.Killer.UserId && !(ev.Player.TeamRole.Team == ev.Killer.TeamRole.Team))
            {
                foreach (Player player in ntf)
                {
                    if (player.UserId == ev.Killer.UserId)
                    {
                        ntfKills += 1;
                    }
                }
                foreach (Player player in chaos)
                {
                    if (player.UserId == ev.Killer.UserId)
                    {
                        chaosKills += 1;
                    }
                }
                ev.SpawnRagdoll = false;
            }
        }

        float timer = 0;
        float inbetweenTimer = 0;
        float secondTimer = 0;

        bool inbetweenTimerActivated = false;
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
                    string str = "<color=#07d100>Winner: </color>";
                    if (ntfKills > chaosKills)
                    {
                        str += "<color=#0000ff>NTF Team</color> <color=#07d100>with " + ntfKills + " kill(s)!</color>";
                    }
                    else if (chaosKills > ntfKills)
                    {
                        str += "<color=#00ff00>Chaos Team</color> <color=#07d100>with " + chaosKills + " kill(s)!</color>";
                    }
                    else if (chaosKills == ntfKills)
                    {
                        str = "<color=#07d100>Its a TIE!</color>";
                    }

                    foreach (Player player in players)
                    {
                        try
                        {
                            player.ChangeRole(Smod2.API.RoleType.TUTORIAL, true, false);
                            player.PersonalBroadcast(10, str, false);
                        }
                        catch { }
                    }
                    plugin.Round.RoundLock = false;
                }
                if (inbetweenTimer > inbetweenTime && !inbetweenTimerActivated)
                {
                    extraText = "";
                    inbetweenTimerActivated = true;
                    inbetweenTime = 20;
                    inbetweenTimer = 0;
                }
                else if (inbetweenTimer > inbetweenTime)
                {
                    for (int i = 0; i <= 35; i++)
                        foreach (Smod2.API.Item item in plugin.Server.Map.GetItems((Smod2.API.ItemType)i, true))
                            item.Remove();
                    inbetweenTime = 20;
                    inbetweenTimer = 0;
                }
                if (secondTimer < 1)
                {
                    secondTimer += Time.deltaTime;
                }
                else if (!gameOver)
                {
                    secondTimer = 0;
                    try
                    {
                        plugin.Info("1");
                        foreach (Player player in GetPlayers())
                        {
                            if (player.OverwatchMode == false)
                            {
                                string str = "";
                                str = "<color=#0000ff>NTF Kills: " + ntfKills + "</color> | <color=#00ff00>Chaos Kills: " + chaosKills + "</color> | Time left: " + (time - timer).ToString("0") + "\n" + extraText;
                                player.PersonalBroadcast(1, str, false);
                            }
                            plugin.Info("2");
                            if (player.TeamRole.Role == Smod2.API.RoleType.SPECTATOR && player.OverwatchMode == false)
                            {
                                System.Random rnd = new System.Random();
                                plugin.Info("3");
                                foreach (Player playerNTF in ntf)
                                {
                                    if (playerNTF.UserId == player.UserId)
                                    {
                                        player.ChangeRole(Smod2.API.RoleType.NTF_LIEUTENANT, true, false);
                                        int pos = rnd.Next(0, posDoors.Count);
                                        player.Teleport(new Vector(posDoors[pos].Position.x, posDoors[pos].Position.y + 1, posDoors[pos].Position.z));
                                        foreach (Smod2.API.Item item in player.GetInventory())
                                            item.Remove();
                                        player.GiveItem(Smod2.API.ItemType.USP);
                                        player.GiveItem(Smod2.API.ItemType.P90);
                                        player.GiveItem(Smod2.API.ItemType.MP7);
                                        player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                                        player.GiveItem(Smod2.API.ItemType.COM15);
                                        player.GiveItem(Smod2.API.ItemType.O5_LEVEL_KEYCARD);
                                        player.GiveItem(Smod2.API.ItemType.WEAPON_MANAGER_TABLET);
                                        player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                                        player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                                        player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                                        player.GiveItem(Smod2.API.ItemType.MEDKIT);
                                    }
                                }
                                foreach (Player playerChaos in chaos)
                                {
                                    if (playerChaos.UserId == player.UserId)
                                    {
                                        player.ChangeRole(Smod2.API.RoleType.CHAOS_INSURGENCY, true, false);
                                        int pos = rnd.Next(0, posDoors.Count);
                                        player.Teleport(new Vector(posDoors[pos].Position.x, posDoors[pos].Position.y + 1, posDoors[pos].Position.z));
                                        foreach (Smod2.API.Item item in player.GetInventory())
                                            item.Remove();
                                        player.GiveItem(Smod2.API.ItemType.USP);
                                        player.GiveItem(Smod2.API.ItemType.P90);
                                        player.GiveItem(Smod2.API.ItemType.MP7);
                                        player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                                        player.GiveItem(Smod2.API.ItemType.COM15);
                                        player.GiveItem(Smod2.API.ItemType.O5_LEVEL_KEYCARD);
                                        player.GiveItem(Smod2.API.ItemType.WEAPON_MANAGER_TABLET);
                                        player.SetAmmo(Smod2.API.AmmoType.AMMO556, 1000);
                                        player.SetAmmo(Smod2.API.AmmoType.AMMO762, 1000);
                                        player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 1000);
                                        player.GiveItem(Smod2.API.ItemType.MEDKIT);
                                    }
                                }
                            }
                        }
                        plugin.Info("7");
                    }
                    catch { plugin.Info("=== Error 1"); }
                }
            }
        }
    }
}