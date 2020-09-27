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
    class DClassInvasionEventHandler : IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerFixedUpdate, IEventHandlerRoundStart
    {
        private readonly FrideysEventBundle plugin;

        public DClassInvasionEventHandler(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        private bool eventRunning = false;
        float time;
        float inbetweenTime;

        public void OnRoundStart(RoundStartEvent ev)
        {
            try
            {
                if (plugin.eventQeue[0] == "dclassinvasion")
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

        public bool BeginEvent()
        {
            if (GetPlayers().Count >= 4)
            {
                List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
                foreach (Smod2.API.Door door in doors)
                {
                    if (door.Name == "ESCAPE_INNER")
                    {
                        door.Open = false;
                        door.Locked = true;
                    }
                    if (door.Name == "SURFACE_GATE")
                    {
                        door.Open = false;
                    }
                }
                plugin.Round.RoundLock = true;
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    try { lift.Locked = true; }
                    catch { plugin.Debug(lift.ToString() + " is not lockable."); }
                }
                plugin.Info("1");
                if (GetPlayers().Count < 7)
                {
                    List<Player> players = GetPlayers();
                    System.Random rnd = new System.Random();
                    SpawnAllAsSpec(players);
                    plugin.Info("2");
                    int ntf1 = rnd.Next(0, players.Count);
                    players[ntf1].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    ClearPlayerInventory(players[ntf1]);
                    plugin.Info("3");
                    GiveItems(players[ntf1], 0, 6);
                    players[ntf1].SetHealth(5000);
                    plugin.Info("4");
                    SpawnDClass(players);
                    time = 360;
                }
                else if (7 <= GetPlayers().Count && GetPlayers().Count < 13)
                {
                    List<Player> players = GetPlayers();
                    System.Random rnd = new System.Random();
                    SpawnAllAsSpec(players);
                    plugin.Info("2");
                    int ntf1 = rnd.Next(0, 7);
                    int ntf2 = rnd.Next(7, 13);
                    players[ntf1].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    players[ntf2].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    ClearPlayerInventory(players[ntf1]);
                    ClearPlayerInventory(players[ntf2]);
                    plugin.Info("3");
                    GiveItems(players[ntf1], 0, 7);
                    GiveItems(players[ntf2], 0, 7);
                    players[ntf1].SetHealth(5000);
                    players[ntf2].SetHealth(5000);
                    plugin.Info("4");
                    SpawnDClass(players);
                    time = 420;
                }
                else
                {
                    List<Player> players = GetPlayers();
                    System.Random rnd = new System.Random();
                    SpawnAllAsSpec(players);
                    plugin.Info("2");
                    int ntf1 = rnd.Next(0, 7);
                    int ntf2 = rnd.Next(7, 13);
                    int ntf3 = rnd.Next(13, players.Count);
                    players[ntf1].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    players[ntf2].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    players[ntf3].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    ClearPlayerInventory(players[ntf1]);
                    ClearPlayerInventory(players[ntf2]);
                    ClearPlayerInventory(players[ntf3]);
                    plugin.Info("3");
                    GiveItems(players[ntf1], 0, 8);
                    GiveItems(players[ntf2], 0, 8);
                    GiveItems(players[ntf3], 0, 8);
                    players[ntf1].SetHealth(5000);
                    players[ntf2].SetHealth(5000);
                    players[ntf3].SetHealth(5000);
                    plugin.Info("4");
                    SpawnDClass(players);
                    time = 480;
                }
                plugin.Info("all spawned");
                inbetweenTime = 120f;
                eventRunning = true;
                plugin.sendmassagediscord("__**D Class Invasion Event started...**__", "502152907509202945");
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
            winCondition = false;
            inbetweenTimerActivated = false;
            inbetweenTimer = 0;
        }

        float timer;
        float inbetweenTimer;
        float secondTimer;

        bool inbetweenTimerActivated = false;
        bool winCondition = false;
        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (eventRunning)
            {
                if (timer < time)
                {
                    timer += Time.deltaTime;
                    inbetweenTimer += Time.deltaTime;
                }
                else
                {
                    winCondition = true;
                    foreach (Player player in GetPlayers())
                    {
                        player.ChangeRole(Smod2.API.RoleType.TUTORIAL, true, false);
                    }
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(10, "<color=#0000ff>NTF win!</color>", false);
                    plugin.Round.RoundLock = false;
                }
                if (inbetweenTimer > inbetweenTime && !inbetweenTimerActivated)
                {
                    inbetweenTimer = 0;
                    Vector vec = new Vector(182.3f, 994, -59.3f);
                    Vector rot = new Vector(0, 0, 0);
                    for (int i = 0; i < 5; i++)
                    {
                        plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO556, vec, rot);
                        plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO762, vec, rot);
                        plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO9MM, vec, rot);
                    }
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FRAG_GRENADE, vec, rot);
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FRAG_GRENADE, vec, rot);
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FLASHBANG, vec, rot);
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FLASHBANG, vec, rot);
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(10, "Supplies at helicopter dropoff!", false);
                }
                if (secondTimer < 1)
                {
                    secondTimer += Time.deltaTime;
                }
                else
                {
                    if (!winCondition)
                    {
                        int ntfLeft = 0;
                        foreach (Player player in GetPlayers())
                        {
                            if (player.TeamRole.Role == Smod2.API.RoleType.NTF_COMMANDER)
                            {
                                ntfLeft += 1;
                            }
                            if (player.TeamRole.Role == Smod2.API.RoleType.SPECTATOR && player.OverwatchMode == false)
                            {
                                RespawnDClass(player);
                            }
                        }
                        if (ntfLeft < 1)
                        {
                            winCondition = true;
                            plugin.Server.Map.Broadcast(10, "<color=#00ff00>Class Ds win!</color>", false);
                            plugin.Round.RoundLock = false;
                        }
                    }
                    secondTimer = 0;
                }
            }
        }

        public List<Player> GetPlayers()
        {
            return plugin.Server.GetPlayers();
        }

        void RespawnDClass(Player player)
        {
            player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
            System.Random rnd = new System.Random();
            int place = rnd.Next(0, 2);
            switch (place)
            {
                default:
                    player.Teleport(new Vector(-11, 1002, -43));
                    break;
                case 1:
                    player.Teleport(new Vector(-11, 1002, -43));
                    break;
                case 0:
                    player.Teleport(new Vector(10.6f, 989, -48.6f));
                    break;
            }
            player.GiveItem(Smod2.API.ItemType.COM15);
            player.GiveItem(Smod2.API.ItemType.MP7);
            player.SetAmmo(AmmoType.AMMO9MM, 100);
            player.SetAmmo(AmmoType.AMMO762, 75);
            player.PersonalBroadcast(10, "Kill the NTF!! They are VERY tough! You have " + ((time - timer) / 60).ToString("0.0") + " minutes", false);
        }

        void SpawnAllAsSpec(List<Player> players)
        {
            foreach (Player player in players)
            {
                player.ChangeRole(Smod2.API.RoleType.SPECTATOR);
            }
        }

        private void ClearPlayerInventory(Player player)
        {
            foreach (Smod2.API.Item item in player.GetInventory())
                item.Remove();
        }

        void GiveItems(Player player, int playerClass/*1=dboi|0=ntf*/, int timeMin)
        {
            switch (playerClass)
            {
                default:
                    player.GiveItem(Smod2.API.ItemType.GUN_MP7);
                    player.GiveItem(Smod2.API.ItemType.GUN_PROJECT90);
                    player.GiveItem(Smod2.API.ItemType.GUN_USP);
                    player.GiveItem(Smod2.API.ItemType.E11_STANDARD_RIFLE);
                    player.GiveItem(Smod2.API.ItemType.COM15);
                    player.SetAmmo(AmmoType.AMMO556, 200);
                    player.SetAmmo(AmmoType.AMMO762, 200);
                    player.SetAmmo(AmmoType.AMMO9MM, 200);
                    player.PersonalBroadcast(15, "You have 5000 HP, survive for " + timeMin + " minutes.", false);
                    break;
                case 1:
                    player.GiveItem(Smod2.API.ItemType.COM15);
                    player.GiveItem(Smod2.API.ItemType.MP7);
                    player.SetAmmo(AmmoType.AMMO9MM, 100);
                    player.SetAmmo(AmmoType.AMMO762, 75);
                    player.PersonalBroadcast(10, "Kill the NTF!! They are VERY tough! You have " + timeMin + " minutes", false);
                    break;
            }

        }

        void SpawnDClass(List<Player> players)
        {
            plugin.Info("ttt");
            foreach (Player player in players)
            {
                if (player.TeamRole.Role != Smod2.API.RoleType.NTF_COMMANDER && player.OverwatchMode == false)
                {
                    player.ChangeRole(Smod2.API.RoleType.CLASSD, true, false);
                    player.Teleport(new Vector(-11, 1002, -43));
                }
            }
        }
    }
}
