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
    public class ChaosVsNTFEventHandler : IEventHandlerPlayerJoin, IEventHandlerPlayerDie, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerFixedUpdate, IEventHandlerRoundStart
    {
        private readonly FrideysEventBundle plugin;

        public ChaosVsNTFEventHandler(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        private bool eventRunning = false;
        float time = 300f;

        public void OnRoundStart(RoundStartEvent ev)//check if this event is 1st in qeue
        {
            try
            {
                if (plugin.eventQeue[0] == "chaosvsntf")
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

        public bool BeginEvent()//start the event
        {
            List<Player> players = GetPlayers();
            if (players.Count >= 2)
            {
                List<Elevator> lifts = plugin.Server.Map.GetElevators();
                foreach (Elevator lift in lifts)
                {
                    try { lift.Locked = true; }
                    catch { plugin.Debug(lift.ToString() + " is not lockable."); }
                }
                
                int half = players.Count / 2;
                for (int i = 0; i < half; i++)//get one half of players and make em chaos
                {
                    players[i].ChangeRole(Smod2.API.RoleType.CHAOS_INSURGENCY);
                    players[i].SetHealth(150);
                    ClearPlayerInventory(players[i]);
                    GiveItems(players[i]);
                    players[i].PersonalBroadcast(7, "<color=#FF0000>Objective: </color><color=#FF7C00>Kill all NTF!</color>", false);
                    players[i].PersonalBroadcast(7, "<color=#B00000>Deathmatch will occur in 5 minutes, you wont be warned!</color>", false);
                }
                for (int i = half; i < players.Count; i++)//get other half of players and make em ntf
                {
                    players[i].ChangeRole(Smod2.API.RoleType.NTF_COMMANDER);
                    ClearPlayerInventory(players[i]);
                    GiveItems(players[i]);
                    players[i].PersonalBroadcast(7, "<color=#FF0000>Objective: </color><color=#FF7C00>Kill all Chaos Insurgents!</color>", false);
                    players[i].PersonalBroadcast(7, "<color=#B00000>Sudden death will occur in 5 minutes, you wont be warned!</color>", false);
                }
                plugin.Round.RoundLock = false;
                eventRunning = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)// new players that join are put in OW
        {
            if (eventRunning)
            {
                ev.Player.OverwatchMode = true;
                ev.Player.PersonalBroadcast(20, "And event is currently underway and you are spectating.", false);
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)// dead players are put in OW
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

        void EndEvent()//end the event
        {
            foreach (Player player in GetPlayers())
            {
                player.OverwatchMode = false;
            }
            eventRunning = false;
            suddenDeathStarted = false;
            timer = 0;
        }

        float timer;
        bool suddenDeathStarted = false;

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            if (eventRunning && !suddenDeathStarted)//count down till sudden death
            {
                if (timer < time)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    StartSuddenDeath();
                }
            }
        }

        public List<Player> GetPlayers()
        {
            return plugin.Server.GetPlayers();
        }

        void StartSuddenDeath()
        {
            suddenDeathStarted = true;
            List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
            foreach (Smod2.API.Door door in doors)
            {
                if (door.Name == "SURFACE_GATE")
                {
                    door.Open = false;
                    door.Locked = true;
                    break;
                }
            }
            foreach (Player player in GetPlayers())
            {
                if (player.TeamRole.Team == Smod2.API.TeamType.NINETAILFOX)
                {
                    player.Teleport(new Vector(-11.5f, 1002, -20.3f));
                }
                else if (player.TeamRole.Team == Smod2.API.TeamType.CHAOS_INSURGENCY)
                {
                    player.Teleport(new Vector(12.5f, 1002, 1.3f));
                }
                player.PersonalBroadcast(7, "<color=#FF0000>Sudden death!</color>", false);
            }
        }

        private void ClearPlayerInventory(Player player)//clear player inventory, if you couldnt tell :)
        {
            foreach (Smod2.API.Item item in player.GetInventory())
                item.Remove();
        }

        private void GiveItems(Player player)//give everyone the same items
        {
            player.GiveItem(Smod2.API.ItemType.GUN_MP7);
            player.GiveItem(Smod2.API.ItemType.GUN_PROJECT90);
            player.GiveItem(Smod2.API.ItemType.MEDKIT);
            player.GiveItem(Smod2.API.ItemType.FRAG_GRENADE);
            player.GiveItem(Smod2.API.ItemType.GUN_COM15);
            player.SetAmmo(AmmoType.AMMO762, 200);
            player.SetAmmo(AmmoType.AMMO9MM, 200);
        }
    }
}
