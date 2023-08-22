using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RounderRectBaker
{
#if UNITY_EDITOR
    using UnityEditor;

    public class RoundedRectEditor : EditorWindow
    {
        public enum RectType
        {
            Fill = 0,
            Border = 1,
        }

        public static readonly int PropIdBorderRadius = Shader.PropertyToID("_BorderRadius");
        public static readonly int PropIdWidth = Shader.PropertyToID("_Width");
        public static readonly int PropIdMargin = Shader.PropertyToID("_Margin");
        public static readonly int PropIdSoftness = Shader.PropertyToID("_Softness");
        public static readonly int PropIdColor = Shader.PropertyToID("_Color");
        public static readonly int PropIdRatio = Shader.PropertyToID("_Ratio");

        private static RoundedRectEditor editor = null;

        private Material materialFill = null;
        private Material materialBorder = null;

        private RectType rectType = RectType.Fill;
        private Vector2Int imageSize = new Vector2Int(512, 512);
        private float borderRadius = 0.1f;
        private float width = 0.025f;
        private float margin = 0.1f;
        private float softness = 0.01f;
        private Color color = Color.white;
        private Vector2 ratio = Vector2.one;
        private RenderTexture renderTexture = null;

        [MenuItem("RoundedRectBaker/Open Editor")]
        private static void ShowEditorWindowAsSingle()
        {
            if(editor == null)
            {
                editor = EditorWindow.CreateWindow<RoundedRectEditor>();
            }
            editor?.Show();
            editor?.Init();
        }

        private void Init()
        {
            materialFill = Resources.Load<Material>("Materials/RoundedRect");
            materialBorder = Resources.Load<Material>("Materials/RoundedRectBorder");
            
            Debug.Assert(materialFill != null);
            Debug.Assert(materialBorder != null);

            CreateBuffer();
        }

        private void CreateBuffer()
        {
            renderTexture = new RenderTexture(imageSize.x, imageSize.y, 0, RenderTextureFormat.ARGB32);
            renderTexture.enableRandomWrite = true;
        }

        private Material UpdateMaterial(RectType rectType)
        {
            Material material = null;
            switch (rectType)
            {
                case RectType.Fill:
                    material = materialFill;
                    break;
                case RectType.Border:
                    material = materialBorder;
                    break;
            }

            if(material == null)
            {
                throw new System.Exception();
            }

            material.SetFloat(PropIdBorderRadius, borderRadius);
            material.SetFloat(PropIdWidth, width);
            material.SetFloat(PropIdMargin, margin);
            material.SetFloat(PropIdSoftness, softness);
            material.SetColor(PropIdColor, color);
            material.SetVector(PropIdRatio, ratio);

            return material;
        }

        private void BakeTexture(ref RenderTexture dstTexture, ref Material material)
        {
            var prev = RenderTexture.active;
            //RenderTexture.active = dstTexture;
            Debug.Log(material.name);
            Graphics.Blit(Resources.Load<Texture2D>("shigotoneko_chikisho"), dstTexture, material);
            RenderTexture.active = dstTexture;
            RenderTexture.active = prev;
        }

        private void OnGUI()
        {
            imageSize = EditorGUILayout.Vector2IntField("Image Size", imageSize);
            if(GUILayout.Button("Create Buffer"))
            {
                CreateBuffer();
            }
            EditorGUILayout.Space();

            rectType = (RectType)EditorGUILayout.EnumPopup("Rect Type", rectType);
            borderRadius = EditorGUILayout.Slider("Border Radius", borderRadius, 0.0f, 0.5f);
            width = EditorGUILayout.Slider("Width", width, 0.0f, 0.5f);
            margin = EditorGUILayout.Slider("Margin", margin, 0.0f, 0.5f);
            softness = EditorGUILayout.Slider("Softness", softness, 0.0f, 0.5f);
            color = EditorGUILayout.ColorField("Color", color);
            ratio = EditorGUILayout.Vector2Field("Ratio", ratio);

            var material = UpdateMaterial(rectType);
            BakeTexture(ref renderTexture, ref material);

            if (renderTexture != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Preview:");
                GUILayout.Label(renderTexture);
            }
        }
    }
#endif
}