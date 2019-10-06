using UnityEngine;

namespace Game.Scripts.Effects
{
    public class ScrollingMaterial : MonoBehaviour
    {
        public Material material;
        public float scrollingSpeed1 = 1;
        public bool alsoDetailTexture = false;
        public float scrollingSpeed2 = 1;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int DetailAlbedoMap = Shader.PropertyToID("_DetailAlbedoMap");

        private void OnDestroy()
        {
            // Prevent Unity material change in editor, reset before game stops
            ResetOffset();
        }

        void ResetOffset()
        {
            material.SetTextureOffset(MainTex, new Vector2(0, 0));
            if (alsoDetailTexture)
            {
                material.SetTextureOffset(DetailAlbedoMap, new Vector2(0, 0));
            }
        }

        void Update()
        {
            //Debug.Log(string.Join(",", material.GetTexturePropertyNames()));
            material.SetTextureOffset(MainTex, new Vector2(0, Time.time * scrollingSpeed1));
            if (alsoDetailTexture)
            {
                material.SetTextureOffset(DetailAlbedoMap,
                    new Vector2(Time.time * scrollingSpeed2, Time.time * scrollingSpeed2));
            }
        }
    }
}
