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
            jacob.BlockPermanentEvents = true;
            jacob.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER", 0);
            vehicle.Mods.InstallModKit();
            vehicle.Mods.PrimaryColor = VehicleColor.WornDarkRed;
            spawned = true;
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
        }

        public void DeleteJacob()
        {
            Jacob.MarkAsNoLongerNeeded();
            Vehicle.MarkAsNoLongerNeeded();
            JacobSpawnpoint.JacobModel.MarkAsNoLongerNeeded();
            JacobSpawnpoint.CarModel.MarkAsNoLongerNeeded();
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
            } else
            {
                trunk.Open();
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
