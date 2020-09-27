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
    class PeanutpocalypseEventHandler : IEventHandlerPlayerJoin, IEventHandlerPlayerDie, IEventHandlerRoundEnd, IEventHandlerFixedUpdate, IEventHandlerRoundRestart, IEventHandlerRoundStart
    {
        private readonly FrideysEventBundle plugin;

        public PeanutpocalypseEventHandler(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        private bool eventRunning = false;
        float time = 180f;

        public void OnRoundStart(RoundStartEvent ev)//check if first in qeue
        {
            try
            {
                if (plugin.eventQeue[0] == "peanutpocalypse")
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
            List<Player> players = GetPlayers();
            if (players.Count >= 4)
            {
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    List<Vector> vecs = lift.GetPositions();
                    foreach (Vector vec in vecs)
                    {
                        if (vec.y > 990)
                        {
                            lift.Locked = true;
                            foreach (Vector vec2 in vecs)
                            {
                                if (vec2.y < -900)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        plugin.Server.Map.SpawnItem(Smod2.API.ItemType.MICROHID, new Vector(vec2.x, vec2.y + 3, vec2.z), new Vector(0, 0, 0));
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                for (int i = 0; i < players.Count; i++)
                {
                    switch (i)
                    {
                        case 1:
                            players[i].ChangeRole(Smod2.API.RoleType.SCP_173);
                            players[i].PersonalBroadcast(7, "<color=#FF0000>Kill D-Class to duplicate yourself!</color>", false);
                            break;
                        default:
                            players[i].ChangeRole(Smod2.API.RoleType.CLASSD);
                            players[i].GiveItem(Smod2.API.ItemType.KEYCARD_ZONE_MANAGER);
                            players[i].PersonalBroadcast(10, "<color=#FF8383>You have a keycard, so go to the </color><color=#FF0000>Opened Gates </color><color=#FF8383>to get Micro-HIDs and kill the peanuts! </color>", false);
                            players[i].PersonalBroadcast(5, "<color=#FF0000>Dont die or you'll become one!</color>", false);
                            break;
                    }
                }
                foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
                {
                    if (door.Name == "GATE_A" || door.Name == "GATE_B")
                    {
                        door.Open = true;
                        door.Locked = true;
                    }
                }
                time = 180f;
                eventRunning = true;
                plugin.Round.RoundLock = false;
                plugin.sendmassagediscord("__**Peanutpocalypse Event started...**__", "502152907509202945");
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
            if (eventRunning && ev.Killer.TeamRole.Team == TeamType.CLASSD)
            {
                ev.Player.Teleport(new Vector(ev.Killer.GetPosition().x, ev.Killer.GetPosition().y + 1, ev.Killer.GetPosition().z));
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
            eventRunning = false;
            gatesOpen = false;
            timer = 0;
        }

        float timer;
        float secondTimer;
        bool gatesOpen = false;

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (eventRunning)
            {
                if (timer < time && !gatesOpen)
                {
                    timer += Time.deltaTime;
                }
                else if (!gatesOpen)
                {
                    List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
                    foreach (Smod2.API.Door door in doors)
                    {
                        if (door.Name == "NUKE_ARMORY" || door.Name == "LCZ_ARMORY" || door.Name == "914" || door.Name == "HCZ_ARMORY" || door.Name == "096" || door.Name == "106_BOTTOM" || door.Name == "106_PRIMARY" || door.Name == "106_SECONDARY" || door.Name == "079_FIRST" || door.Name == "079_SECOND" || door.Name == "049_ARMORY" || door.Name == "012")
                        {
                            door.Open = true;
                            door.Locked = true;
                        }
                    }
                    gatesOpen = true;
                }

                if (secondTimer < 1)
                {
                    secondTimer += Time.deltaTime;
                }
                else
                {
                    foreach (Player player in GetPlayers())
                    {
                        if (player.OverwatchMode == false && player.TeamRole.Role != Smod2.API.RoleType.CLASSD && player.TeamRole.Role != Smod2.API.RoleType.SCP_173)
                        {
                            player.ChangeRole(Smod2.API.RoleType.SCP_173, true, false);
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
    }
}
