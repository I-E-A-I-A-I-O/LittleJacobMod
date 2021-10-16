using System;
using GTA;
using GTA.Math;
using LittleJacobMod.Loading;

namespace LittleJacobMod.Utils
{
    internal class LittleJacob
    {
        Vehicle vehicle;
        Ped jacob;
        public Ped Jacob => jacob;
        public Vehicle Vehicle => vehicle;

        public LittleJacob(Vector3 jacobPosition, float jacobHeading, Vector3 vehiclePosition, float vehicleHeading)
        {
            jacob = World.CreatePed(JacobSpawnpoint.JacobModel, jacobPosition, jacobHeading);
            vehicle = World.CreateVehicle(JacobSpawnpoint.CarModel, vehiclePosition, vehicleHeading);
            vehicle.Mods.InstallModKit();
            vehicle.Mods.PrimaryColor = VehicleColor.WornDarkRed;
        }
    }
}
