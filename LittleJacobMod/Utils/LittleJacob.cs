using System;
using GTA;
using GTA.Native;
using LittleJacobMod.Loading;

namespace LittleJacobMod.Utils
{
    public class LittleJacob
    {
        private Vehicle vehicle;
        private Ped jacob;
        private JacobSpawnpoint jacobSpawnpoint;
        private bool spawned, left;
        public bool Spawned => spawned;
        public bool Left => left;
        public Blip Blip { get; }
        public Ped Jacob => jacob;
        public Vehicle Vehicle => vehicle;
        public JacobSpawnpoint JacobSpawnpoint => jacobSpawnpoint;
        public static event EventHandler<bool> TrunkStateChanged;

        public LittleJacob(JacobSpawnpoint jacobSpawnpoint)
        {
            this.jacobSpawnpoint = jacobSpawnpoint;
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

            jacob = World.CreatePed(new Model(Main.JacobHash), jacobSpawnpoint.JacobPosition, jacobSpawnpoint.JacobHeading);

            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, (uint)Main.JacobHash);
            Function.Call(Hash.REQUEST_MODEL, (uint)Main.JacobsCarHash);

            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, (uint)Main.JacobsCarHash))
            {
                Script.Wait(50);
            }

            vehicle = World.CreateVehicle(new Model(Main.JacobsCarHash), jacobSpawnpoint.CarPosition, jacobSpawnpoint.CarHeading);
            Function.Call(Hash.SET_MODEL_AS_NO_LONGER_NEEDED, (uint)Main.JacobsCarHash);
            jacob.BlockPermanentEvents = true;
            jacob.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER", 0);
            vehicle.Mods.InstallModKit();
            vehicle.Mods.PrimaryColor = VehicleColor.WornDarkRed;
            vehicle.Mods.SecondaryColor = VehicleColor.WornDarkRed;
            spawned = true;
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
            jacob.Task.PerformSequence(sequence);
            sequence.Dispose();
            left = true;
            Timers.RestartOfferSmokeTimer();
        }

        public void Terminate()
        {
            left = true;
            Timers.RestartOfferSmokeTimer();
        }

        public void DeleteJacob()
        {
            Jacob.MarkAsNoLongerNeeded();
            Vehicle.MarkAsNoLongerNeeded();
        }

        public bool IsPlayerInArea()
        {
            return Game.Player.Character.IsInRange(jacobSpawnpoint.JacobPosition, 80);
        }

        public bool IsNearby()
        {
            return jacob.IsInRange(Game.Player.Character.Position, 50);
        }

        public bool PlayerNearTrunk()
        {
            return Game.Player.Character.IsInRange(Vehicle.RearPosition, 1.2f);
        }

        public void ToggleTrunk()
        {
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
            if (Blip == null)
            {
                return;
            }
            Blip.Delete();
        }
    }
}
