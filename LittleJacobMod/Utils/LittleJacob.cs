using System;
using GTA;
using GTA.Native;
using LittleJacobMod.Loading;

namespace LittleJacobMod.Utils
{
    public class LittleJacob
    {
        public bool Spawned { get; private set; }

        public bool Left { get; private set; }

        public Blip Blip { get; }
        public Ped? Jacob { get; private set; }

        public Vehicle? Vehicle { get; private set; }

        public JacobSpawnpoint JacobSpawnpoint { get; private set; }

        public static event EventHandler<bool>? TrunkStateChanged;

        public LittleJacob(JacobSpawnpoint jacobSpawnpoint)
        {
            JacobSpawnpoint = jacobSpawnpoint;
            Blip = World.CreateBlip(jacobSpawnpoint.JacobPosition);
            Blip.Sprite = BlipSprite.Lester;
            Blip.Color = BlipColor.Green;
            Blip.Name = "Little Jacob";
            Blip.IsShortRange = false;
        }

        public void Spawn()
        {
            Function.Call(Hash.REQUEST_MODEL, (uint)Main.JacobHash);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, (uint)Main.JacobHash))
            {
                Script.Wait(50);
            }

            Jacob = World.CreatePed(new Model(Main.JacobHash), JacobSpawnpoint.JacobPosition, JacobSpawnpoint.JacobHeading);

            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, (uint)Main.JacobHash);
            Function.Call(Hash.REQUEST_MODEL, (uint)Main.JacobsCarHash);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, (uint)Main.JacobsCarHash))
            {
                Script.Wait(50);
            }

            Vehicle = World.CreateVehicle(new Model(Main.JacobsCarHash), JacobSpawnpoint.CarPosition, JacobSpawnpoint.CarHeading);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, (uint)Main.JacobsCarHash);
            Jacob.BlockPermanentEvents = true;
            Jacob.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER", 0);
            Vehicle.Mods.InstallModKit();
            Vehicle.Mods.PrimaryColor = VehicleColor.WornDarkRed;
            Vehicle.Mods.SecondaryColor = VehicleColor.WornDarkRed;
            Spawned = true;
        }

        /*public void ProcessVoice(bool bought = false, bool force = false)
        {
            if (!spawned || Voice.Playing)
            {
                return;
            }

            if (!HelloPlayed && Game.Player.Character.IsInRange(Jacob.Position, 8))
            {
                Voice.PlayHello();
                HelloPlayed = true;
            }

            if (!ByePlayed && Left && IsNearby() && Jacob.IsAlive && Game.Player.WantedLevel == 0)
            {
                Voice.PlayBye();
                ByePlayed = true;
            }

            if (!ByePlayed && Left && Jacob.IsAlive && Game.Player.WantedLevel > 0)
            {
                Voice.PlayPolice();
                ByePlayed = true;
            }

            if (!SmokeOffered && !Left && Main.MenuOpened && Timers.OfferSmoke() && Ran.Next(1, 101) <= 6)
            {
                Voice.PlaySmokeOffer();
                SmokeOffered = true;
            }

            if (bought)
            {
                if (force)
                {
                    Voice.PlayPurchase();
                } else if (Ran.Next(1, 101) <= 30)
                {
                    Voice.PlayPurchase();
                }
            }
        }*/

        public void DriveAway()
        {
            Blip.Delete();
            var sequence = new TaskSequence();
            sequence.AddTask.ClearAll();
            sequence.AddTask.CruiseWithVehicle(Vehicle, 100);
            sequence.Close();
            Jacob?.Task.PerformSequence(sequence);
            sequence.Dispose();
            Left = true;
            Timers.RestartOfferSmokeTimer();
        }

        public void Terminate()
        {
            Left = true;
            Timers.RestartOfferSmokeTimer();
        }

        public void DeleteJacob()
        {
            Jacob?.MarkAsNoLongerNeeded();
            Vehicle?.MarkAsNoLongerNeeded();
        }

        public bool IsPlayerInArea()
        {
            return Game.Player.Character.IsInRange(JacobSpawnpoint.JacobPosition, 80);
        }

        public bool IsNearby()
        {
            return Jacob?.IsInRange(Game.Player.Character.Position, 50) ?? false;
        }

        public bool PlayerNearTrunk()
        {
            return Vehicle != null && Game.Player.Character.IsInRange(Vehicle.RearPosition, 1.2f);
        }

        public void ToggleTrunk()
        {
            if (Vehicle == null) return;
            var trunk = Vehicle.Doors[VehicleDoorIndex.Trunk];
            if (trunk.IsOpen)
            {
                trunk.Close();
                TrunkStateChanged?.Invoke(this, false);
            } else
            {
                trunk.Open();
                TrunkStateChanged?.Invoke(this, true);
            }
        }

        public void DeleteBlip()
        {
            if (Blip.Handle == 0)
            {
                return;
            }
            Blip.Delete();
        }
    }
}
