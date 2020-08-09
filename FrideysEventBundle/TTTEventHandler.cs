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
    class TTTEventHandler : IEventHandlerPlayerDropItem, IEventHandlerPlayerPickupItem, IEventHandlerPlayerJoin, IEventHandlerPlayerLeave, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerShoot, IEventHandlerFixedUpdate, IEventHandlerRoundStart, IEventHandler079Door
    {
        private readonly FrideysEventBundle plugin;

        public TTTEventHandler(FrideysEventBundle plugin)
        {
            this.plugin = plugin;
        }

        private bool eventRunning = false;
        float time;
        float inbetweenTime;
        public List<Player> tttPlayers = new List<Player>();

        public void OnRoundStart(RoundStartEvent ev)
        {
            try
            {
                if (plugin.eventQeue[0] == "ttt")
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
                    if (door.Name == "914" || door.Name == "CHECKPOINT_LCZ_A" || door.Name == "CHECKPOINT_LCZ_B" || door.Name == "LCZ_ARMORY" || door.Name == "012")
                    {
                        door.Open = false;
                        door.Locked = true;
                    }
                }
                Player murderer;
                plugin.Info("1");
                Player sherif;
                plugin.Info("2");
                List<Player> innocents = GetPlayers();
                System.Random rnd = new System.Random();
                int murderInt = rnd.Next(0, innocents.Count);
                murderer = innocents[murderInt];
                innocents.Remove(murderer);
                plugin.Info("3");
                int shrifInt = rnd.Next(0, innocents.Count);
                sherif = innocents[shrifInt];
                innocents.Remove(sherif);
                plugin.Info("4");
                tttPlayers.Add(murderer);
                tttPlayers.Add(sherif);
                plugin.Info("5");
                murderer.ChangeRole(Smod2.API.RoleType.CLASSD);
                murderer.PersonalBroadcast(10, "<color=#ff0000>You are the murderer</color>\n<color=#ff8f8f>You will recieve your weapon in 30 seconds.</color>", false);
                murderer.PersonalBroadcast(20, "<color=#ff8f8f>You must kill everyone, but beware of the sherif who can kill you. You have 7 minutes to kill everyone!</color>", false);
                sherif.ChangeRole(Smod2.API.RoleType.CLASSD);
                sherif.PersonalBroadcast(10, "<color=#ffdd00>You are the Sherif</color>\n<color=#fff199>You will recieve your gun in 30 seconds.</color>", false);
                sherif.PersonalBroadcast(20, "<color=#fff199>You must kill the murderer, but beware if you kill the wrong person, youll die!</color>", false);
                plugin.Info("6");
                foreach (Player player in innocents)
                {
                    player.ChangeRole(Smod2.API.RoleType.CLASSD);
                    player.PersonalBroadcast(10, "<color=#00ff00>You are an Innocent</color>\n<color=#a3ffa3>You must survive for 7 minutes.</color>", false);
                    player.PersonalBroadcast(20, "<color=#a3ffa3>If the sherif dies, you can pick up their gun and become the new sherif.</color>", false);
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

        public void OnPlayerDropItem(PlayerDropItemEvent ev)
        {
            if (eventRunning)
            {
                ev.Allow = false;
            }
        }

        public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
        {
            if (eventRunning)
            {
                if (ev.Player.UserId != tttPlayers[0].UserId && ev.Item.ItemType == Smod2.API.ItemType.USP)
                {
                    tttPlayers.Add(ev.Player);
                    ev.Player.SetAmmo(Smod2.API.AmmoType.AMMO9MM, 500);
                    ev.Player.PersonalBroadcast(10, "<color=#ffdd00>You are now the Sherif</color>\n<color=#fff199>You must kill the murderer, but beware if you kill the wrong person, youll die!</color>", false);
                }
                else
                {
                    ev.Allow = false;
                }
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

        public void OnPlayerLeave(PlayerLeaveEvent ev)
        {
            if (eventRunning)
            {
                if (ev.Player.UserId == tttPlayers[0].UserId)
                {
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(10, "<color=#00ff00>Innocents win!</color>", false);
                    plugin.Round.RoundLock = false;
                }
                else if (ev.Player == tttPlayers[1])
			    {
                    plugin.Server.Map.SpawnItem(Smod2.API.ItemType.USP, new Vector(ev.Player.GetPosition().x, ev.Player.GetPosition().y + 1, ev.Player.GetPosition().z), new Vector(0, 0, 0));
                    tttPlayers.RemoveAt(1);
                    plugin.Server.Map.Broadcast(10, "<color=#ff0000>The sherif has died! Find the gun to become the next sherif!</color>", false);
                }
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
            deletedItems = false;
        }

        public void OnShoot(PlayerShootEvent ev)
        {
            if (eventRunning)
            {
                if (ev.Target != null)
                {
                    if (ev.Player.UserId == tttPlayers[1].UserId && ev.Target.UserId != tttPlayers[0].UserId)
                    {
                        plugin.Server.Map.SpawnItem(Smod2.API.ItemType.USP, new Vector(ev.Player.GetPosition().x, ev.Player.GetPosition().y + 1, ev.Player.GetPosition().z), new Vector(0, 0, 0));
                        ev.Player.ChangeRole(Smod2.API.RoleType.SPECTATOR);
                        ev.Player.OverwatchMode = true;
                        tttPlayers.RemoveAt(1);
                        ev.Target.ChangeRole(Smod2.API.RoleType.SPECTATOR);
                        ev.Target.OverwatchMode = true;
                        plugin.Server.Map.Broadcast(10, "<color=#ff0000>The sherif has died! Find the gun to become the next sherif!</color>", false);
                    }
                    else if (ev.Target.UserId == tttPlayers[0].UserId && ev.Player.UserId == tttPlayers[1].UserId)
                    {
                        ev.Target.ChangeRole(Smod2.API.RoleType.SPECTATOR);
                        plugin.Server.Map.ClearBroadcasts();
                        plugin.Server.Map.Broadcast(10, "<color=#00ff00>Innocents win!</color>", false);
                        plugin.Round.RoundLock = false;
                    }
                    else
                    {
                        ev.Target.ChangeRole(Smod2.API.RoleType.SPECTATOR);
                        ev.Target.OverwatchMode = true;
                    }
                }
            }
        }

        float timer;
        float inbetweenTimer;
        float secondTimer;

        bool inbetweenTimerActivated = false;

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
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(10, "<color=#00ff00>Innocents win!</color>", false);
                    plugin.Round.RoundLock = false;
                }
                if (inbetweenTimer > inbetweenTime && !inbetweenTimerActivated)
                {
                    try
                    {
                        tttPlayers[0].GiveItem(Smod2.API.ItemType.COM15);
                        tttPlayers[1].GiveItem(Smod2.API.ItemType.USP);
                        tttPlayers[0].SetAmmo(Smod2.API.AmmoType.AMMO9MM, 500);
                        tttPlayers[1].SetAmmo(Smod2.API.AmmoType.AMMO9MM, 500);
                        inbetweenTimerActivated = true;
                    }
                    catch
                    {
                        plugin.Info("error");
                    }
                }
                if (secondTimer < 1)
                {
                    secondTimer += Time.deltaTime;
                }
                else
                {
                    int innocentsLeft = 0;
                    foreach (Player player in GetPlayers())
                    {
                        if (player.UserId != tttPlayers[0].UserId && player.TeamRole.Role == Smod2.API.RoleType.CLASSD)
                        {
                            innocentsLeft += 1;
                        }
                    }
                    if (innocentsLeft < 1)
                    {
                        plugin.Server.Map.ClearBroadcasts();
                        plugin.Server.Map.Broadcast(10, "<color=#ff0000>The Murderer wins!</color>", false);
                        plugin.Round.RoundLock = false;
                    }
                    secondTimer = 0;
                }
            }
        }

        public List<Player> GetPlayers()
        {
            return plugin.Server.GetPlayers();
        }

        bool deletedItems = false;

        public void On079Door(Player079DoorEvent ev)
        {
            if (eventRunning && !deletedItems)
            {
                for (int i = 0; i <= 35; i++)
                    foreach (Smod2.API.Item item in plugin.Server.Map.GetItems((Smod2.API.ItemType)i, true))
                        item.Remove();
                deletedItems = true;
            }
        }
    }
}
