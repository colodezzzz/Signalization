using System;
using UnityEditor.Presets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Recorder
{
    static class FindObjectsHelper
    {
        internal static T[] FindObjectsByTypeWrapper<T>(
#if UNITY_2023_1_OR_NEWER
            FindObjectsSortMode sortMode = FindObjectsSortMode.None
#endif
        ) where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<T>(sortMode);
#else
            return UnityEngine.Object.FindObjectsOfType<T>();
#endif
        }
    }

    static class PresetHelper
    {
        static Texture2D s_PresetIcon;
        static GUIStyle s_PresetButtonStyle;

        internal static Texture2D presetIcon
        {
            get
            {
                if (s_PresetIcon == null)
                    s_PresetIcon = (Texture2D)EditorGUIUtility.Load(EditorGUIUtility.isProSkin ? "d_Preset.Context@2x" : "Preset.Context@2x");

                return s_PresetIcon;
            }
        }

        internal static GUIStyle presetButtonStyle
        {
            get
            {
                return s_PresetButtonStyle ?? (s_PresetButtonStyle = new GUIStyle("iconButton") { fixedWidth = 19.0f });
            }
        }

        internal static void ShowPresetSelectorWrapper(RecorderSettings settings, Preset currentSelection = null,
            Action onSelectionChanged = null, Action onSelectionClosed = null)
        {
#if UNITY_2023_1_OR_NEWER

            Action<Preset> OnSelectionChangedIgnoreParams = _ =>
            {
                if (onSelectionChanged != null) onSelectionChanged();
            };

            Action<Preset, bool> OnSelectionClosedIgnoreParams = (_, _) =>
            {
                if (onSelectionClosed != null) onSelectionClosed();
            };

            PresetSelector.ShowSelector(new UnityEngine.Object[] { settings }, currentSelection, true, OnSelectionChangedIgnoreParams, OnSelectionClosedIgnoreParams);
        }

#else
            var presetReceiver = ScriptableObject.CreateInstance<PresetReceiver>();
            presetReceiver.Init(settings, onSelectionChanged, onSelectionClosed);

            PresetSelector.ShowSelector(settings, currentSelection, true, presetReceiver);
        }

        internal class PresetReceiver : PresetSelectorReceiver
        {
            RecorderSettings m_Target;
            Preset m_InitialValue;
            Action m_OnSelectionChanged;
            Action m_OnSelectionClosed;

            internal void Init(RecorderSettings target, Action onSelectionChanged = null, Action onSelectionClosed = null)
            {
                m_OnSelectionChanged = onSelectionChanged;
                m_OnSelectionClosed = onSelectionClosed;
                m_Target = target;
                m_InitialValue = new Preset(target);
            }

            public override void OnSelectionChanged(Preset selection)
            {
                if (selection != null)
                {
                    Undo.RecordObject(m_Target, "Apply Preset " + selection.name);
                    selection.ApplyTo(m_Target);
                }
                else
                {
                    Undo.RecordObject(m_Target, "Cancel Preset");
                    m_InitialValue.ApplyTo(m_Target);
                }

                if (m_OnSelectionChanged != null)
                    m_OnSelectionChanged.Invoke();
            }

            public override void OnSelectionClosed(Preset selection)
            {
                OnSelectionChanged(selection);

                m_Target.OnAfterDuplicate();

                if (m_OnSelectionClosed != null)
                    m_OnSelectionClosed.Invoke();

                DestroyImmediate(this);
            }
        }
#endif
    }
}
