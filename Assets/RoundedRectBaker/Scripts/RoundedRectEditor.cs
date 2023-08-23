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
        public static readonly int PropIdRectType = Shader.PropertyToID("_RECT_TYPE");

        private static RoundedRectEditor editor = null;

        private Material material = null;

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
            material = Resources.Load<Material>("Materials/RoundedRect");
            CreateBuffer();
        }

        private void CreateBuffer()
        {
            renderTexture = new RenderTexture(imageSize.x, imageSize.y, 0, RenderTextureFormat.ARGB32);
            renderTexture.enableRandomWrite = true;
        }

        private Material UpdateMaterial(RectType rectType)
        {
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
            material.SetFloat(PropIdRectType, (int)rectType);

            return material;
        }

        private void BakeTexture(ref RenderTexture dstTexture, ref Material material)
        {
            var prev = RenderTexture.active;
            Graphics.Blit(Texture2D.whiteTexture, dstTexture, material);
            RenderTexture.active = prev;
        }

        private void SaveTexture(ref RenderTexture texture)
        {
            var prev = RenderTexture.active;
            RenderTexture.active = texture;
            var texture2d = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            texture2d.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture2d.Apply();
            RenderTexture.active = prev;

            string filePath = EditorUtility.SaveFilePanel("Save Texture", "", "RoundedRect", "png");
            if (!string.IsNullOrEmpty(filePath))
            {
                System.IO.File.WriteAllBytes(filePath, texture2d.EncodeToPNG());
            }
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
            if (rectType == RectType.Border)
            {
                width = EditorGUILayout.Slider("Width", width, 0.0f, 0.5f);
                margin = EditorGUILayout.Slider("Margin", margin, 0.0f, 0.5f);
            }
            else
            {
                width = 0f;
                margin = 0f;
            }
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

            if (GUILayout.Button("Save Texture"))
            {
                SaveTexture(ref renderTexture);
            }
        }
    }
#endif
}