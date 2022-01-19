using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using LittleJacobMod.Utils;

namespace LittleJacobMod.Loading
{
    static internal class Initialize
    {
        private static List<JacobSpawnpoint> jacobSpawnpoints = new List<JacobSpawnpoint>()
        {
            new JacobSpawnpoint(new Vector3(1625.764f, 1002.065f, 104.7569f), -163.9208f, new Vector3(1627.689f, 1003.179f, 104.6205f), -14.01786f),
            new JacobSpawnpoint(new Vector3(700.1344f, 222.0037f, 92.46917f), 91.96848f, new Vector3(699.8804f, 219.7696f, 91.87753f), -118.9912f),
            new JacobSpawnpoint(new Vector3(-25.51013f, -229.6167f, 46.17864f), -160.0232f, new Vector3(-23.39594f, -228.6828f, 45.63602f), -12.67927f),
            new JacobSpawnpoint(new Vector3(-465.946f, -927.0502f, 23.68365f), 124.3571f, new Vector3(-464.6559f, -929.0339f, 23.14076f), -78.74215f),
            new JacobSpawnpoint(new Vector3(-1583.922f, -1026.3f, 7.618188f), -71.31367f, new Vector3(-1584.287f, -1024.226f, 7.048428f), 61.80254f),
            new JacobSpawnpoint(new Vector3(-1817.975f, 805.7697f, 138.6255f), -27.80928f, new Vector3(-1820.419f, 806.8063f, 138.1721f), 114.3365f),
            new JacobSpawnpoint(new Vector3(-2688.357f, 2378.431f, 4.574349f), -67.40932f, new Vector3(-2689.402f, 2380.829f, 3.612689f), 85.9256f),
            new JacobSpawnpoint(new Vector3(-2178.969f, 4270.593f, 49.0836f), -165.7394f, new Vector3(-2177.015f, 4271.835f, 48.50634f), -12.11151f),
            new JacobSpawnpoint(new Vector3(-1583.362f, 5159.921f, 19.56199f), -161.9957f, new Vector3(-1580.357f, 5160.372f, 19.08035f), -22.86525f),
            new JacobSpawnpoint(new Vector3(83.43481f, 6329.314f, 31.22223f), 64.64294f, new Vector3(82.68629f, 6327.746f, 30.69057f), -139.8963f),
            new JacobSpawnpoint(new Vector3(1683.079f, 6436.93f, 32.18724f), -129.6566f, new Vector3(1684.56f, 6438.635f, 31.74157f), 27.36946f),
            new JacobSpawnpoint(new Vector3(2619.262f, 4014.977f, 42.53349f), -159.1368f, new Vector3(2621.41f, 4016.096f, 42.36407f), -18.64159f),
            new JacobSpawnpoint(new Vector3(1726.618f, 3297.503f, 41.22349f), -100.8708f, new Vector3(1726.824f, 3299.747f, 40.68145f), 52.67164f),
            new JacobSpawnpoint(new Vector3(490.3468f, 2457.75f, 48.88317f), 88.86375f, new Vector3(491.2083f, 2455.765f, 48.17788f), -116.5646f),
            new JacobSpawnpoint(new Vector3(-308.7968f, -769.286f, 38.77979f), 48.75363f, new Vector3(-310.9363f, -770.2584f, 38.23925f), -176.8784f),
            new JacobSpawnpoint(new Vector3(-18.61436f, -1080.107f, 26.67206f), 171.6731f, new Vector3(-16.3138f, -1079.976f, 26.13109f), -46.15764f),
            new JacobSpawnpoint(new Vector3(638.8298f, -607.9145f, 14.61435f), -116.1156f, new Vector3(639.0124f, -605.6197f, 14.11228f), 53.32741f),
            new JacobSpawnpoint(new Vector3(1197.671f, -1265.096f, 35.22675f), -109.8357f, new Vector3(1198.265f, -1263.107f, 34.68498f), 23.19619f),
            new JacobSpawnpoint(new Vector3(1537.972f, -1538.455f, 76.27715f), 117.647f, new Vector3(1539.33f, -1540.654f, 75.55095f), -89.82295f),
            new JacobSpawnpoint(new Vector3(2567.646f, -682.6725f, 54.17775f), 37.20398f, new Vector3(2566f, -682.9736f, 53.83439f), -178.0244f),
            new JacobSpawnpoint(new Vector3(2593.172f, 494.1709f, 108.4827f), 88.21005f, new Vector3(2592.86f, 491.6433f, 107.965f), -133.6859f),
            new JacobSpawnpoint(new Vector3(2330.546f, 2439.053f, 64.20835f), 44.999f, new Vector3(2327.062f, 2439.063f, 63.93856f), 142.1617f),
            new JacobSpawnpoint(new Vector3(2678.141f, 3527.897f, 52.42685f), 41.08424f, new Vector3(2676.414f, 3527.486f, 51.89222f), -166.8861f),
            new JacobSpawnpoint(new Vector3(1724.354f, 4804.543f, 41.67359f), 117.8686f, new Vector3(1725.49f, 4802.246f, 41.17456f), -108.6089f)
        };

        private static JacobSpawnpoint CurrentSpawnpoint { get; set; }

        public static LittleJacob CalculateClosestSpawnpoint()
        {
            var closestPoint = jacobSpawnpoints[0];
            var currentDistance = World.CalculateTravelDistance(Game.Player.Character.Position, closestPoint.CarPosition);
            foreach (JacobSpawnpoint spawnpoint in jacobSpawnpoints)
            {
                if (Game.Player.Character.IsInRange(spawnpoint.CarPosition, 65))
                {
                    continue;
                }

                var distance = World.CalculateTravelDistance(Game.Player.Character.Position, spawnpoint.CarPosition);
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    closestPoint = spawnpoint;
                }
            }
            CurrentSpawnpoint = closestPoint;
            return new LittleJacob(CurrentSpawnpoint);
        }
    }
}
