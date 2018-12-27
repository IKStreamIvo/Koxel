using Koxel.World;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using SimplexNoise;
using System;

namespace Koxel.Modding{
    public class JSONCalc{
        public float result;

        public JSONCalc(JToken calc, int x = 0, int y = 0){
            string type = calc["type"].ToObject<string>();
            JToken args = calc["args"];
            switch(type){
                //Math operators
                case "+": case "add": {
                    float[] values = GetNumbers(args.ToObject<object[]>());
                    result = values[0];
                    for (int i = 1; i < values.Length; i++){
                        result += values[i];
                    }
                    break;
                }

                case "-": case "sub": case "subtract": {
                    float[] values = GetNumbers(args.ToObject<object[]>());
                    result = values[0];
                    for (int i = 1; i < values.Length; i++){
                        result -= values[i];
                    }
                    break;
                }

                case "*": case "mult": case "multiply": {
                    float[] values = GetNumbers(args.ToObject<object[]>());
                    result = values[0];
                    for (int i = 1; i < values.Length; i++){
                        result *= values[i];
                    }
                    break;
                }

                case "/": case "div": case "divide": {
                    float[] values = GetNumbers(args.ToObject<object[]>());
                    result = values[0];
                    for (int i = 1; i < values.Length; i++){
                        result /= values[i];
                    }
                    break;
                }

                //Math functions
                case "pow": {
                    float[] values = GetNumbers(args.ToObject<object[]>());
                    result = Mathf.Pow(values[0], values[1]);
                    break;
                }

                case "sqrt": {
                    result = Mathf.Sqrt(GetNumber(args.ToObject<object>()));
                    break;
                }
                
                //Noise functions
                case "simplex": case "noise": {
                    float[] values = GetNumbers(args.ToObject<object[]>());
                    result = IslandGenerator.Simplex.Evaluate(x / values[0], y / values[1]);
                    break;
                } 
            }
        }

        public float[] GetNumbers(object[] objs){
            float[] result = new float[objs.Length];
            for (int i = 0; i < objs.Length; i++){
                result[i] = GetNumber(objs[i]);
            }
            return result;
        }

        public float GetNumber(object obj){
            if(obj is float || obj is double || obj is long || obj is int){
                return Convert.ToSingle(obj);
            }
            else if(obj is JToken){
                JToken jToken = JToken.Parse(obj.ToString());
                JSONCalc val = new JSONCalc(jToken);
                return val.result;
            }
            else if(obj is string){
                Game.Error(Game.Systems.ModManager, "Strings are not supported as an input value for calculations.");
                return 0f;
            }
            else{
                Game.Error(Game.Systems.ModManager, "Type '" + obj.GetType() + "' is unknown/unsupported.");
                return 0f;
            }
        }
    }
}