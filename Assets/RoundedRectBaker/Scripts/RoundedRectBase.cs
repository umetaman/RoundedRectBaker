using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RounderRectBaker
{
    public abstract class RoundedRectBase : MonoBehaviour
    {
        public static readonly int PropIdBorderRadius = Shader.PropertyToID("_BorderRadius");
        public static readonly int PropIdWidth = Shader.PropertyToID("_Width");
        public static readonly int PropIdMargin = Shader.PropertyToID("_Margin");
        public static readonly int PropIdSoftness = Shader.PropertyToID("_Softness");
        public static readonly int PropIdColor = Shader.PropertyToID("_Color");
        public static readonly int PropIdRatio = Shader.PropertyToID("_Ratio");

        protected abstract Material LoadMaterial();
        
        protected void Bake(ref Material material, ref RenderTexture dstTexture)
        {
            Graphics.Blit(dstTexture, material);
        }
    }
}