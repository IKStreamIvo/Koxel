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

        public static Vector2 CubeToAxial(Vector3 hex)
        {
            return new Vector2(hex.x, hex.z);
        }

        public static Vector3 AxialToCube(float q, float r)
        {
            return new Vector3(q, -q-r, r);
        }

        public static Vector3 AxialToCube(Vector2 hex)
        {
            return new Vector3(hex.x, -hex.x-hex.y, hex.y);
        }

        public static Vector3 PixelToHex(float x, float y, HexData hexData)
        {
            int q = (int)((x * Math.Sqrt(3) / 3 - y / 3) / hexData.Size);
            int r = (int)(y * 2f / 3f / hexData.Size);
            Vector3 coords = AxialToCube(q, r);
            return coords;
        }

        public static Vector2 HexAdd(Vector2 a, Vector2 b){
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 HexSubtract(Vector2 a, Vector2 b){
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 HexMultiply(Vector2 a, Vector2 b){
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        public static Vector3 HexAdd(Vector3 a, Vector3 b){
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 HexSubtract(Vector3 a, Vector3 b){
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 HexMultiply(Vector3 a, Vector3 b){
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static int HexLength(Vector3 hex){
            return (int)((Math.Abs(hex.x) + Math.Abs(hex.y) + Math.Abs(hex.z)) / 2f);
        }

        public static int HexLength(Vector2 hex){
            Vector3 hexy = AxialToCube(hex);
            return (int)((Math.Abs(hexy.x) + Math.Abs(hexy.y) + Math.Abs(hexy.z)) / 2f);
        }

        public static float HexDistance(Vector2 a, Vector2 b){
            return HexLength(HexSubtract(a, b));
        }
    }
}