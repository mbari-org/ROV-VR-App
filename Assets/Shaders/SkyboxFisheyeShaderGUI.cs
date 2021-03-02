using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace UnityEditor
{
    internal class SkyboxFisheyeShaderGUI : ShaderGUI
    {
        readonly AnimBool m_ShowLatLongLayout = new AnimBool();
        readonly AnimBool m_ShowFisheyeLayout = new AnimBool();
        readonly AnimBool m_ShowMirrorOnBack = new AnimBool();
        readonly AnimBool m_Show3DControl = new AnimBool();

        bool m_Initialized = false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            if (!m_Initialized)
            {
                m_ShowLatLongLayout.valueChanged.AddListener(materialEditor.Repaint);
                m_ShowFisheyeLayout.valueChanged.AddListener(materialEditor.Repaint);
                m_ShowMirrorOnBack.valueChanged.AddListener(materialEditor.Repaint);
                m_Show3DControl.valueChanged.AddListener(materialEditor.Repaint);
                m_Initialized = true;
            }

            // Allow the default implementation to set widths for consistency for common properties.
            float lw = EditorGUIUtility.labelWidth;
            materialEditor.SetDefaultGUIWidths();
            ShowProp(materialEditor, FindProperty("_Tint", props));
            ShowProp(materialEditor, FindProperty("_Exposure", props));
            ShowProp(materialEditor, FindProperty("_Rotation", props));

            EditorGUIUtility.labelWidth = lw;

            float layout = ShowProp(materialEditor, FindProperty("_Mapping", props));
            m_ShowLatLongLayout.target = layout == 1;
            m_ShowFisheyeLayout.target = layout == 2;


            if (EditorGUILayout.BeginFadeGroup(m_ShowFisheyeLayout.faded))
            {
                // Fisheye requires two separate images               
                ShowProp(materialEditor, FindProperty("_LTex", props));
                ShowProp(materialEditor, FindProperty("_RTex", props));

                // 2D/3D Toggle
                ShowProp(materialEditor, FindProperty("_Mode", props));

                // Fisheye Settings
                ShowProp(materialEditor, FindProperty("_L_CX", props));
                ShowProp(materialEditor, FindProperty("_L_CY", props));
                ShowProp(materialEditor, FindProperty("_L_RX", props));
                ShowProp(materialEditor, FindProperty("_L_RY", props));

                ShowProp(materialEditor, FindProperty("_R_CX", props));
                ShowProp(materialEditor, FindProperty("_R_CY", props));
                ShowProp(materialEditor, FindProperty("_R_RX", props));
                ShowProp(materialEditor, FindProperty("_R_RY", props));

                ShowProp(materialEditor, FindProperty("_a", props));
                ShowProp(materialEditor, FindProperty("_b", props));
                ShowProp(materialEditor, FindProperty("_c", props));
            } else
            {
                // Other layouts expect one stereo image
                ShowProp(materialEditor, FindProperty("_Tex", props));
            }

            if (EditorGUILayout.BeginFadeGroup(m_ShowLatLongLayout.faded))
            {
                m_ShowMirrorOnBack.target = ShowProp(materialEditor, FindProperty("_ImageType", props)) == 1;
                if (EditorGUILayout.BeginFadeGroup(m_ShowMirrorOnBack.faded))
                {
                    EditorGUI.indentLevel++;
                    ShowProp(materialEditor, FindProperty("_MirrorOnBack", props));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();
                //// No 3D settings unless PlayerSettings have VR support.
                //m_Show3DControl.value = PlayerSettings.virtualRealitySupported;
                //if (EditorGUILayout.BeginFadeGroup(m_Show3DControl.faded))
                //    ShowProp(materialEditor, FindProperty("_Layout", props));
                //EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();


            EditorGUILayout.EndFadeGroup();

            // Let the default implementation add the extra shader properties at the bottom.
            materialEditor.PropertiesDefaultGUI(new MaterialProperty[0]);
        }

        private float ShowProp(MaterialEditor materialEditor, MaterialProperty prop)
        {
            materialEditor.ShaderProperty(prop, prop.displayName);
            return prop.floatValue;
        }
    }
}
