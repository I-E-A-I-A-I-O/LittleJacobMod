using System;
using GTA;
using GTA.Math;
using LittleJacobMod.Loading;

namespace LittleJacobMod.Utils
{
    public class LittleJacob
    {
        Vehicle vehicle;
        Ped jacob;
        JacobSpawnpoint jacobSpawnpoint;
        bool spawned, left;
        public bool Spawned => spawned;
        public bool Left => left;
        public Blip Blip { get; }
        public Ped Jacob => jacob;
        public Vehicle Vehicle => vehicle;
        public JacobSpawnpoint JacobSpawnpoint => jacobSpawnpoint;
        bool HelloPlayed { get; set; }
        bool ByePlayed { get; set; }
        bool SmokeOffered { get; set; }
        Random Ran => new Random();
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
            JacobSpawnpoint.JacobModel.Request();
            JacobSpawnpoint.CarModel.Request();
            var modelsLoaded = false;
            while (!modelsLoaded)
            {
                Script.Wait(0);
                modelsLoaded = JacobSpawnpoint.JacobModel.IsLoaded && JacobSpawnpoint.CarModel.IsLoaded;
            }
            jacob = World.CreatePed(JacobSpawnpoint.JacobModel, jacobSpawnpoint.JacobPosition, jacobSpawnpoint.JacobHeading);
            vehicle = World.CreateVehicle(JacobSpawnpoint.CarModel, jacobSpawnpoint.CarPosition, jacobSpawnpoint.CarHeading);
            Script.Wait(0);
            JacobSpawnpoint.JacobModel.MarkAsNoLongerNeeded();
            JacobSpawnpoint.CarModel.MarkAsNoLongerNeeded();
            jacob.BlockPermanentEvents = true;
            jacob.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER", 0);
            vehicle.Mods.InstallModKit();
            vehicle.Mods.PrimaryColor = VehicleColor.WornDarkRed;
            spawned = true;
        }

        public void ProcessVoice(bool bought = false, bool force = false)
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
        }

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
