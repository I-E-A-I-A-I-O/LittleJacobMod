using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace LittleJacobMod.Loading
{
    public class JacobSpawnpoint
    {
        Vector3 jacobPosition;
        Vector3 carPosition;
        float carHeading;
        float jacobHeading;
        public static Model CarModel => new Model(VehicleHash.Manana);
        public static Model JacobModel => new Model(PedHash.Soucent03AMY);
        public Vector3 JacobPosition => jacobPosition;
        public Vector3 CarPosition => carPosition;
        public float JacobHeading => jacobHeading;
        public float CarHeading => carHeading;

        public JacobSpawnpoint(Vector3 jacobPosition, float jacobHeading, Vector3 carPosition, float carHeading)
        {
            this.jacobPosition = jacobPosition;
            this.carHeading = carHeading;
            this.jacobHeading = jacobHeading;
            this.carPosition = carPosition;
        }
    }
}
