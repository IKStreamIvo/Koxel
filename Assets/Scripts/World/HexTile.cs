using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koxel.World
{
    public class HexTile : MonoBehaviour
    {
        public Renderer top;
        public Renderer border;
        public List<Renderer> parts;

        public Vector3 coords;
        public Chunk chunk;

        public string biome;
        public string tileType;

        public float size;

        private MaterialPropertyBlock _propBlock;
        private MeshRenderer _renderer;
        private MeshCollider _collider;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<MeshRenderer>();

            biome = "hi";
            tileType = "cutie";
        }

        public void SetSize(float height)
        {
            size = height;
            parts[0].transform.localScale = new Vector3(1f, Mathf.Abs(height) * 20f, 1f);
        }

        public void SetColor(Color top, Color border, List<Color> parts)
        {
            float trandom = 0f;//Random.Range(-0.05f, 0.05f);
            Color tColor = new Color(top.r + trandom, top.g + trandom, top.b + trandom, top.a);
            this.top.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", tColor);
            this.top.SetPropertyBlock(_propBlock);

            float brandom = 0f;//Random.Range(-0.05f, 0.05f);
            Color bColor = new Color(border.r + brandom, border.g + brandom, border.b + brandom, border.a);
            this.border.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", bColor);
            this.border.SetPropertyBlock(_propBlock);

            foreach(Color color in parts){
                float random = 0f;//Random.Range(-0.05f, 0.05f);
                Color newColor = new Color(color.r + random, color.g + random, color.b + random, color.a);
                this.parts[parts.IndexOf(color)].GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_Color", newColor);
                this.parts[parts.IndexOf(color)].SetPropertyBlock(_propBlock);
            }
        }
    }
}
