using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(KeySetActionData))]
public class KeySetActionDataDrawer : PropertyDrawer
{
    //アニメーションキー設定用配列の画面表示を設定
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            position.height = EditorGUIUtility.singleLineHeight;

            var actionTypeProperty = property.FindPropertyRelative("keySetActionType");

            switch((KeySetActionType)actionTypeProperty.enumValueIndex)
            {
                case KeySetActionType.KeySet:
                    {
                        //キー位置の項目を表示
                        var percent_KeySetRect = new Rect(position)
                        {
                            y = position.y
                        };
                        var percentProperty = property.FindPropertyRelative("percent");
                        percentProperty.intValue = EditorGUI.IntField(percent_KeySetRect, "キー位置（%）",percentProperty.intValue);

                        //unlitの項目を表示
                        var unlit_value_Rect = new Rect(percent_KeySetRect)
                        {
                            y = percent_KeySetRect.y + EditorGUIUtility.singleLineHeight + 2f
                        };

                        var unlit_valueProperty = property.FindPropertyRelative("unlit_value");
                        unlit_valueProperty.floatValue = EditorGUI.FloatField(unlit_value_Rect, "Unlit", unlit_valueProperty.floatValue);

                        //minLimitsの項目を表示
                        var minLimits_Rect = new Rect(unlit_value_Rect)
                        {
                            y = unlit_value_Rect.y + EditorGUIUtility.singleLineHeight + 2f
                        };

                        var minLimits_value_Property = property.FindPropertyRelative("minLimits_value");
                        minLimits_value_Property.floatValue = EditorGUI.FloatField(minLimits_Rect, "minLimits", minLimits_value_Property.floatValue);

                        //maxLimitsの項目を表示
                        var maxLimits_value_Rect = new Rect(minLimits_Rect)
                        {
                            y = minLimits_Rect.y + EditorGUIUtility.singleLineHeight + 2f
                        };

                        var maxLimits_value_value_Property = property.FindPropertyRelative("maxLimits_value");
                        maxLimits_value_value_Property.floatValue = EditorGUI.FloatField(maxLimits_value_Rect, "maxLimits", maxLimits_value_value_Property.floatValue);

                        break;
                    }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //アニメーションキー設定用配列の画面表示の立幅を設定
        var height = EditorGUIUtility.singleLineHeight;

        var actionTypeProperty = property.FindPropertyRelative("keySetActionType");
        switch ((KeySetActionType)actionTypeProperty.enumValueIndex)
        {
            case KeySetActionType.KeySet:
                {
                    height = 90f;
                    break;
                }
        }
        return height;
    }
}
