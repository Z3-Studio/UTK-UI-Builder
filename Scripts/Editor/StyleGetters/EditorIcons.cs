using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Z3.UIBuilder.Editor
{
    public static class EditorIcons
    {
        // See more icons here: https://github.com/halak/unity-editor-icons
        private static Dictionary<IconType, string> IconPaths = new()
        {
            { IconType.Lamp, "PointLight Gizmo" },
            { IconType.Plus, "d_Toolbar Plus@2x" },
            { IconType.Minus, "d_Toolbar Minus@2x" },
            { IconType.Ok, "TestPassed" },  // Installed@2x
            { IconType.Info, "d_console.infoicon@2x" },
            { IconType.Warning, "d_console.warnicon@2x" },
            { IconType.Error, "d_console.erroricon@2x" },
            { IconType.Invisible, "d_SceneViewVisibility@2x" },
            { IconType.Visible, "d_scenevis_visible_hover@2x" },
            { IconType.Box, "d_Package Manager@2x" },
            { IconType.Cog, "d_SettingsIcon@2x" },
            { IconType.Gamepad, "d_UnityEditor.GameView@2x" },
            { IconType.VerticalLayoutGroup, "d_VerticalLayoutGroup Icon" },
            { IconType.ParticleSystemForceField, "ParticleSystemForceField Gizmo" },
            { IconType.AudioMixerController, "AudioMixerController On Icon" },
            { IconType.Eye, "d_ViewToolOrbit On@2x" },
            { IconType.Globo, "d_Profiler.GlobalIllumination@2x" },
            { IconType.Avatar, "d_AvatarSelector@2x" },
            { IconType.Body, "BodySilhouette" },
            { IconType.Cloud, "CloudConnect" },
            { IconType.Store, "Asset Store" },
            { IconType.Pivot, "d_AvatarPivot@2x" },
            { IconType.Play, "PlayButton On@2x" }
        };

        public static Texture GetTexture(IconType iconType)
        {
            return GetGUIContent(iconType).image;
        }
        public static Texture2D GetTexture2D(IconType iconType)
        {
            IconPaths.TryGetValue(iconType, out string iconPath);
            return EditorGUIUtility.FindTexture(iconPath);
        }

        public static GUIContent GetGUIContent(IconType iconType)
        {
            IconPaths.TryGetValue(iconType, out string iconPath);

            if (string.IsNullOrEmpty(iconPath))
                return GUIContent.none;

            return EditorGUIUtility.IconContent(iconPath);
        }

        public static Texture2D GetTypeIcon(Type type)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return AssetPreview.GetMiniTypeThumbnail(type);

            // TODO
            return null;
        }
    }
}