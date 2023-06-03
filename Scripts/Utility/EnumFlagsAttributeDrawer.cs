#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

namespace WatStudios.DeepestDungeon.Utility.Editor
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            prop.intValue = EditorGUI.MaskField(pos, label, prop.intValue, prop.enumNames);
        }
    }
}
#endif