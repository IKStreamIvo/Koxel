using System;
using System.Collections.Generic;
using UnityEngine;

namespace Koxel.Tech
{
    public class HexData
    {
        public float Size { get; private set; }
        public float Height { get; private set; }
        public float Width { get; private set; }

        public HexData(float hexSize)
        {
            Size = hexSize;

            Height = Size * 2f;
            Width = (float)Math.Sqrt(3f) / 2f * Height;
        }
    }

    public static class HexMath
    {
        /*
         * function cube_to_axial(cube):
    var q = cube.x
    var r = cube.z
    return Hex(q, r)

function axial_to_cube(hex):
    var x = hex.q
    var z = hex.r
    var y = -x-z
    return Cube(x, y, z)
    */
        public static Vector2 CubeToAxial(float x, float z)
        {
            return new Vector2(x, z);
        }

        public static Vector3 AxialToCube(float q, float r)
        {
            return new Vector3(q, -q-r, r);
        }

        public static Vector3 PixelToHex(float x, float y, HexData hexData)
        {
            int q = (int)((x * Math.Sqrt(3) / 3 - y / 3) / hexData.Size);
            int r = (int)(y * 2f / 3f / hexData.Size);
            Vector3 coords = AxialToCube(q, r);
            return coords;
        }
    }
}