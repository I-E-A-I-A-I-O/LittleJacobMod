using System;
using GTA.Math;

namespace LittleJacobMod.Loading
{
    public class JacobSpawnpoint
    {
        public Vector3 JacobPosition { get; }

        public Vector3 CarPosition { get; }

        public float JacobHeading { get; }

        public float CarHeading { get; }

        public JacobSpawnpoint(Vector3 jacobPosition, float jacobHeading, Vector3 carPosition, float carHeading)
        {
            JacobPosition = jacobPosition;
            CarHeading = carHeading;
            JacobHeading = jacobHeading;
            CarPosition = carPosition;
        }
    }
}
