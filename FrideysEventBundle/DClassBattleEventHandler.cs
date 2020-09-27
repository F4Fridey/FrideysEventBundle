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
    class DClassBattleEventHandler : IEventHandlerPlayerJoin, IEventHandlerPlayerDie, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerFixedUpdate, IEventHandlerRoundStart
    {
        private readonly FrideysEventBundle plugin;

        public DClassBattleEventHandler(FrideysEventBundle plugin)
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
                if (plugin.eventQeue[0] == "dclassbattle")
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
                plugin.Round.RoundLock = true;
                List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
                foreach (Smod2.API.Door door in doors)
                {
                    if (-100 < door.Position.y && door.Position.y < 100)
                    {
                        System.Random rnd = new System.Random();
                        SpawnItems(rnd, door);
                        SpawnItems(rnd, door);
                        int chanceWhatAmmo = rnd.Next(0, 4);
                        switch (chanceWhatAmmo)
                        {
                            default:
                                break;
                            case 1:
                                plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO762, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                                break;
                            case 2:
                                plugin.Server.Map.SpawnItem(Smod2.API.ItemType.AMMO9MM, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                                break;
                        }
                        if (door.Name == "SURFACE_GATE" || door.Name == "914" || door.Name == "CHECKPOINT_LCZ_A" || door.Name == "CHECKPOINT_LCZ_B" || door.Name == "012")
                        {
                            door.Open = false;
                            door.Locked = true;
                        }
                        else
                        {
                            door.Open = false;
                        }
                    }
                    else if (door.Name == "SURFACE_GATE")
                    {
                        door.Open = false;
                        door.Locked = true;
                    }
                }
                foreach (Player player in GetPlayers())
                {
                    player.ChangeRole(Smod2.API.RoleType.CLASSD);
                    player.PersonalBroadcast(1, "<color=#00ff00>Battle Royal! Find weapons and items to fight till the last man standing! You are invincible for 180 seconds.</color>", false);

                    player.SetGodmode(true);
                }
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    try { lift.Locked = true; }
                    catch { plugin.Debug(lift.ToString() + " is not lockable."); }
                }

                time = 600;
                inbetweenTime = 180f;
                eventRunning = true;
                plugin.sendmassagediscord("**__D Class Battle Event started...__**", "502152907509202945");
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
                ev.Player.OverwatchMode = true;
                ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (eventRunning)
            {
                ev.Player.OverwatchMode = true;
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
                    if (winCondition)
                    {
                        plugin.Round.RoundLock = false;
                    }
                    else
                    {
                        foreach (Player player in GetPlayers())
                        {
                            if (player.TeamRole.Team == Smod2.API.TeamType.CLASSD)
                            {
                                player.Teleport(new Vector(-11.5f, 1002, -20.3f));
                            }
                            player.PersonalBroadcast(3, "<color=#FF0000>Sudden death!</color>", false);
                        }
                    }
                }
                if (inbetweenTimer > inbetweenTime && !inbetweenTimerActivated)
                {
                    foreach (Player player in GetPlayers())
                    {
                        player.SetGodmode(false);
                        player.PersonalBroadcast(15, "<color=#ff0000>Invincibility Over. Last man standing wins!\nSudden death in 7 minutes, you wont be warned!</color>", false);
                    }
                    inbetweenTimerActivated = true;
                }
                if (secondTimer < 1)
                {
                    secondTimer += Time.deltaTime;
                }
                else
                {
                    if (!winCondition)
                    {
                        int dclassLeft = 0;
                        foreach (Player player in GetPlayers())
                        {
                            if (player.TeamRole.Role == Smod2.API.RoleType.CLASSD)
                            {
                                dclassLeft += 1;
                            }
                            if (player.GetGodmode())
                                player.PersonalBroadcast(1, "<color=#00ff00>Battle Royal! Find weapons and items to fight till the last man standing! You are invincible for " + (inbetweenTime - inbetweenTimer).ToString("0") + " seconds.</color>", false);
                        }
                        if (dclassLeft < 2)
                        {
                            winCondition = true;
                            time = 5f;
                            timer = 0;
                            string lastPlayerName = "no one";
                            foreach (Player player in GetPlayers())
                            {
                                if (player.TeamRole.Role == Smod2.API.RoleType.CLASSD) { lastPlayerName = player.Name; }
                            }
                            plugin.Server.Map.Broadcast(10, "<color=#00ff00>Congratulations to " + lastPlayerName + ", the last man standing!</color>", false);
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

        void SpawnItems(System.Random rnd, Smod2.API.Door door)
        {
            int chanceWhatItem = rnd.Next(0, 10);
            switch (chanceWhatItem)
            {
                default:
                    break;
                case 0:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.COM15, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 1:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.MEDKIT, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 2:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.SCP500, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 3:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.SCP207, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 4:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.P90, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 5:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.USP, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 6:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.SCP018, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 7:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FRAG_GRENADE, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 8:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.FLASHLIGHT, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
                case 9:
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.MP7, new Vector(door.Position.x, door.Position.y + 2, door.Position.z), new Vector(0, 0, 0));
                    break;
            }
        }
    }
}
