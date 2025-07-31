using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/AnimationClipSet")]
public class KeySetData : ScriptableObject
{
    //アニメーションキー設定用配列オブジェクト
    public int _tabIndex = 0;
    public int id;
    public List<KeySetActionData> keyActionList = new List<KeySetActionData>()
    {
        new KeySetActionData{keySetActionType = KeySetActionType.KeySet, percent = 0, unlit_value = 0f, minLimits_value = 0f, maxLimits_value = 0f},
        new KeySetActionData{keySetActionType = KeySetActionType.KeySet, percent = 25, unlit_value = 0f, minLimits_value = 0f, maxLimits_value = 0.5f},
        new KeySetActionData{keySetActionType = KeySetActionType.KeySet, percent = 50, unlit_value = 0.1f, minLimits_value = 0.1f, maxLimits_value = 1f},
        new KeySetActionData{keySetActionType = KeySetActionType.KeySet, percent = 100, unlit_value = 0.1f, minLimits_value = 1f, maxLimits_value = 1f}
    };

    public bool writeDefaultValues = false;

    public bool BaseLayer_Copy = false;
    public bool AdditiveLayer_Copy = false;
    public bool GestureLayer_Copy = false;
    public bool ActionLayer_Copy = false;
    public bool FXLayer_Copy = false;
    public bool EXMenu_Copy = false;
    public bool EXPara_Copy = false;
    public bool SittngLayer_Copy = false;
    public bool TPoseLayer_Copy = false;
    public bool IKPoseLayer_Copy = false;

    public float MinLightLimits = 0;
    public float MaxLightLimits = 0;

    public bool LightLimits_ALL = true;
    public bool LightLimits_Save;
    public float LightLimits_Default = 0;
    public float LightLimits_Min = 0;
    public float LightLimits_Max = 1;

    public bool MinLightLimits_Save;
    public float MinLightLimits_Default = 0;
    public float MinLightLimits_Min = 0;
    public float MinLightLimits_Max = 1;

    public bool MaxLightLimits_Save;
    public float MaxLightLimits_Default = 1;
    public float MaxLightLimits_Min = 0;
    public float MaxLightLimits_Max = 1;

    public bool LightLimitsList_Save = true;
    public float LightLimitsList_Default = 0.5f;
}

[Serializable]
public class KeySetActionData
{
    //アニメーションキー設定用オブジェクト
    public KeySetActionType keySetActionType;
    public int percent;
    public float unlit_value;
    public string unlit_key;
    public float minLimits_value;
    public string minLimits_value_key;
    public float maxLimits_value;
    public string maxLimits_value_key;
}

public enum KeySetActionType
{
    KeySet
}
