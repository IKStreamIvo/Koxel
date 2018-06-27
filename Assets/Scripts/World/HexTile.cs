using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koxel.World
{
    public class HexTile : MonoBehaviour
    {
        public Vector3 coords;
        public Chunk chunk;

        public string biome;
        public string tileType;

        private MaterialPropertyBlock _propBlock;
        private SkinnedMeshRenderer _renderer;
        private MeshCollider _collider;

        private void Awake()
        {
            _collider = GetComponentInChildren<MeshCollider>();
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<SkinnedMeshRenderer>();

            biome = "hi";
            tileType = "cutie";
        }

        public void SetSize(float height)
        {
            _collider.transform.localScale = new Vector3(1f, Mathf.Abs(height + 1f) * 2f, 1f);
            _renderer.SetBlendShapeWeight(0, Mathf.Abs(height) * 2f);
        }

        public void SetColor(Color color)
        {
            float random = 0f;//Random.Range(-0.05f, 0.05f);
            Color newColor = new Color(color.r + random, color.g + random, color.b + random, color.a);
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", newColor);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
