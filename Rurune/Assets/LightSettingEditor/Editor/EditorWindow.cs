using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using BBItems.AvatarUtils;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

[CustomEditor(typeof(KeySetData))]
public class EditorWindowSample : EditorWindow
{
    //タブ管理
    private readonly string[] _tabToggles = { "Simple", "Advanced", "CloneLayer" };
    private int _tabIndex;

    public GameObject AvatarObject;
    public bool writeDefaultValues = false;

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

    public AnimationClip MinLightAnimClip;
    public AnimationClip MaxLightAnimClip;

    //シェーダーキーワード（liltoon用）
    const string shaderkeyword_AsUnlit = "material._AsUnlit";
    const string shaderkeyword_LightMinLimit = "material._LightMinLimit";
    const string shaderkeyword_LightMaxLimit = "material._LightMaxLimit";

    public string assetFolderPath = "Assets/LightSettingEditor/";

    //処理結果メッセージ
    public string resultMessage;

    //アニメーションキー設定用配列
    public bool LightLimitsList_Save = true;
    public float LightLimitsList_Default = 0.5f;
    private ReorderableList reorderableList;
    private SerializedProperty keySetActionDataList;
    private SerializedObject serializedObject;
    private KeySetData obj;
    private SerializedProperty id;
    private string keySetDataObjectPath;

    //スクロールバー変数
    private Vector2 _scrollPostition = Vector2.zero;

    private void OnEnable()
    {
        //アニメーションキー設定欄の初期化
        //obj = Resources.Load<KeySetData>(keySetDataObjectPath);
        keySetDataObjectPath = "Assets/LightSettingEditor/" + typeof(KeySetData) + ".asset";
        //keySetDataObjectPath = AssetDatabase.GenerateUniqueAssetPath("Assets/LightSettingEditor/AnimationClipSet.asset");
        obj = AssetDatabase.LoadAssetAtPath<KeySetData>(keySetDataObjectPath);
        if (obj == null)
        {
            obj = ScriptableObject.CreateInstance<KeySetData>();
        }

        serializedObject = new SerializedObject(obj);
        keySetActionDataList = serializedObject.FindProperty("keyActionList");

        reorderableList = new ReorderableList(serializedObject, keySetActionDataList, true, false, true, true);

        reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            var keySetActionData = keySetActionDataList.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, keySetActionData);
        };
        reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "List");
        reorderableList.elementHeightCallback = index => EditorGUI.GetPropertyHeight(keySetActionDataList.GetArrayElementAtIndex(index));
        id = serializedObject.FindProperty("id");

        _tabIndex = serializedObject.FindProperty("_tabIndex").intValue;

        writeDefaultValues = serializedObject.FindProperty("writeDefaultValues").boolValue;

        MinLightLimits = serializedObject.FindProperty("MinLightLimits").floatValue;
        MaxLightLimits = serializedObject.FindProperty("MaxLightLimits").floatValue;

        LightLimits_ALL = serializedObject.FindProperty("LightLimits_ALL").boolValue;
        LightLimits_Save = serializedObject.FindProperty("LightLimits_Save").boolValue;
        LightLimits_Default = serializedObject.FindProperty("LightLimits_Default").floatValue;
        LightLimits_Min = serializedObject.FindProperty("LightLimits_Min").floatValue;
        LightLimits_Max = serializedObject.FindProperty("LightLimits_Max").floatValue;

        MinLightLimits_Save = serializedObject.FindProperty("MinLightLimits_Save").boolValue;
        MinLightLimits_Default = serializedObject.FindProperty("MinLightLimits_Default").floatValue;
        MinLightLimits_Min = serializedObject.FindProperty("MinLightLimits_Min").floatValue;
        MinLightLimits_Max = serializedObject.FindProperty("MinLightLimits_Max").floatValue;

        MaxLightLimits_Save = serializedObject.FindProperty("MaxLightLimits_Save").boolValue;
        MaxLightLimits_Default = serializedObject.FindProperty("MaxLightLimits_Default").floatValue;
        MaxLightLimits_Min = serializedObject.FindProperty("MaxLightLimits_Min").floatValue;
        MaxLightLimits_Max = serializedObject.FindProperty("MaxLightLimits_Max").floatValue;

        LightLimitsList_Save = serializedObject.FindProperty("LightLimitsList_Save").boolValue;
        LightLimitsList_Default = serializedObject.FindProperty("LightLimitsList_Default").floatValue;

        BaseLayer_Copy =  serializedObject.FindProperty("BaseLayer_Copy").boolValue;
        AdditiveLayer_Copy =  serializedObject.FindProperty("AdditiveLayer_Copy").boolValue;
        GestureLayer_Copy =  serializedObject.FindProperty("GestureLayer_Copy").boolValue;
        ActionLayer_Copy =  serializedObject.FindProperty("ActionLayer_Copy").boolValue;
        FXLayer_Copy =  serializedObject.FindProperty("FXLayer_Copy").boolValue;
        EXMenu_Copy =  serializedObject.FindProperty("EXMenu_Copy").boolValue;
        EXPara_Copy = serializedObject.FindProperty("EXPara_Copy").boolValue;
        SittngLayer_Copy = serializedObject.FindProperty("SittngLayer_Copy").boolValue;
        TPoseLayer_Copy = serializedObject.FindProperty("TPoseLayer_Copy").boolValue;
        IKPoseLayer_Copy = serializedObject.FindProperty("IKPoseLayer_Copy").boolValue;

        if (!System.IO.File.Exists(keySetDataObjectPath))
        {
            AssetDatabase.CreateAsset(obj, keySetDataObjectPath);
        }
    }

    [MenuItem("HakuroEditor/LightLimitsSetup")]
	public static void Create()
	{
        GetWindow<EditorWindowSample>("LightLimitsSetup");
	}
    

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.wordWrap = true;

        //共通の表示項目
        GUILayout.Label("アバターのLightSettingアニメーションを設定する。");
        GUILayout.Label("※※※liltoon専用※※※");

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("【処理結果】");
        GUILayout.Label(resultMessage);
        EditorGUILayout.EndVertical();

        //タブ初期化
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabToggles, new GUIStyle(EditorStyles.toolbarButton), GUI.ToolbarButtonSize.FitToContents);
            obj._tabIndex = _tabIndex;
        }
        //アバター設定欄
        AvatarObject = (GameObject)EditorGUILayout.ObjectField("Avatar", AvatarObject, typeof(GameObject), true);

        if (_tabIndex != 2)
        {
            //writeDefault設定
            writeDefaultValues = EditorGUILayout.Toggle("writeDefault", writeDefaultValues);
            obj.writeDefaultValues = writeDefaultValues;
        }
            

        //タブ処理
        if (_tabIndex == 0)
        {
            //設定項目を分岐
            LightLimits_ALL = EditorGUILayout.Toggle("まとめて設定する", LightLimits_ALL);
            obj.LightLimits_ALL = LightLimits_ALL;

            if (LightLimits_ALL)
            {
                //MaximumLightLimitsとMinimumLightLimitsの設定項目を表示
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("LightLimits");
                LightLimits_Save = EditorGUILayout.Toggle("Save", LightLimits_Save);
                obj.LightLimits_Save = LightLimits_Save;
                LightLimits_Default = EditorGUILayout.FloatField("Default", LightLimits_Default);
                obj.LightLimits_Default = LightLimits_Default;
                LightLimits_Min = EditorGUILayout.FloatField("Min", LightLimits_Min);
                obj.LightLimits_Min = LightLimits_Min;
                LightLimits_Max = EditorGUILayout.FloatField("Max", LightLimits_Max);
                obj.LightLimits_Max = LightLimits_Max;
                EditorGUILayout.EndVertical();
            }
            else
            {
                //MinimumLightLimitsの設定項目を表示
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("MinLightLimits");
                MinLightLimits_Save = EditorGUILayout.Toggle("Save", MinLightLimits_Save);
                obj.MinLightLimits_Save = MinLightLimits_Save;
                MinLightLimits_Default = EditorGUILayout.FloatField("Default", MinLightLimits_Default);
                obj.MinLightLimits_Default = MinLightLimits_Default;
                MinLightLimits_Min = EditorGUILayout.FloatField("Min", MinLightLimits_Min);
                obj.MinLightLimits_Min = MinLightLimits_Min;
                MinLightLimits_Max = EditorGUILayout.FloatField("Max", MinLightLimits_Max);
                obj.MinLightLimits_Max = MinLightLimits_Max;
                EditorGUILayout.EndVertical();

                //MaximumLightLimitsの設定項目を表示
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("MaxLightLimits");
                MaxLightLimits_Save = EditorGUILayout.Toggle("Save", MaxLightLimits_Save);
                obj.MaxLightLimits_Save = MaxLightLimits_Save;
                MaxLightLimits_Default = EditorGUILayout.FloatField("Default", MaxLightLimits_Default);
                obj.MaxLightLimits_Default = MaxLightLimits_Default;
                MaxLightLimits_Min = EditorGUILayout.FloatField("Min", MaxLightLimits_Min);
                obj.MaxLightLimits_Min = MaxLightLimits_Min;
                MaxLightLimits_Max = EditorGUILayout.FloatField("Max", MaxLightLimits_Max);
                obj.MaxLightLimits_Max = MaxLightLimits_Max;
                EditorGUILayout.EndVertical();
            }

            //適用ボタンを表示
            if (GUILayout.Button("適用"))
            {
                Debug.Log("START:適用");
                //アバターが設定されているか確認
                if (AvatarObject == null)
                {
                    Debug.Log("Avatarを選択していません。");
                    resultMessage = "Avatarを選択していません。";
                }
                else
                {
                    Debug.Log(AvatarObject.name + "の設定開始。");
                    if (LightLimits_ALL)
                    {
                        LightSettingALLApply(AvatarObject);
                    }
                    else
                    {
                        LightSettingApply(AvatarObject);
                    }

                    EditorUtility.SetDirty(obj);
                    AssetDatabase.SaveAssets();
                    Debug.Log(AvatarObject.name + "の設定終了。");
                }
                Debug.Log("END:適用");
            }

        }
        else if (_tabIndex == 1)
        {
            //適用ボタンを表示
            if (GUILayout.Button("適用"))
            {
                Debug.Log("START:適用");
                //アバターが設定されているか確認
                if (AvatarObject == null)
                {
                    Debug.Log("Avatarを選択していません。");
                    resultMessage = "Avatarを選択していません。";
                }
                else
                {
                    Debug.Log(AvatarObject.name + "の設定開始。");
                    Undo.RecordObject(obj, "Apply LightSetting");
                    //アニメーションキー設定用配列を初期化
                    SerializedProperty listProperty = keySetActionDataList.Copy();
                    List<KeySetActionData> keyActionList = new List<KeySetActionData>();

                    List<int> percentList = new List<int>();
                    List<float> unlit_valueList = new List<float>();
                    List<float> minLimits_valueList = new List<float>();
                    List<float> maxLimits_valueList = new List<float>();

                    if (listProperty.isArray)
                    {
                        //プロパティをループして各変数を個別に取り出し
                        while (listProperty.Next(true))
                        {
                            if (listProperty.propertyType == SerializedPropertyType.Float)
                            {
                                if (listProperty.name == "unlit_value")
                                {
                                    unlit_valueList.Add(listProperty.floatValue);
                                }
                                else if (listProperty.name == "minLimits_value")
                                {
                                    minLimits_valueList.Add(listProperty.floatValue);
                                }
                                else if (listProperty.name == "maxLimits_value")
                                {
                                    maxLimits_valueList.Add(listProperty.floatValue);
                                }
                            }
                            else if (listProperty.propertyType == SerializedPropertyType.Integer)
                            {
                                if (listProperty.name == "percent")
                                {
                                    percentList.Add(listProperty.intValue);
                                }
                            }
                        }

                        //個別に取り出した変数をクラス配列としてまとめる
                        for (int i = 0; i < percentList.Count; i++)
                        {
                            keyActionList.Add(new KeySetActionData { percent = percentList[i], unlit_value = unlit_valueList[i], minLimits_value = minLimits_valueList[i], maxLimits_value = maxLimits_valueList[i] });
                            Debug.Log(percentList[i]);
                        }
                    }
                    
                    LightSettingAdvancedApply(AvatarObject, keyActionList);
                    
                    EditorUtility.SetDirty(obj);
                    AssetDatabase.SaveAssets();
                    Debug.Log(AvatarObject.name + "の設定終了。");
                }
                Debug.Log("END:適用");

            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            LightLimitsList_Save = EditorGUILayout.Toggle("Save", LightLimitsList_Save);
            obj.LightLimitsList_Save = LightLimitsList_Save;
            LightLimitsList_Default = EditorGUILayout.FloatField("Default", LightLimitsList_Default);
            obj.LightLimitsList_Default = LightLimitsList_Default;
            EditorGUILayout.EndVertical();

            _scrollPostition = EditorGUILayout.BeginScrollView(_scrollPostition);

            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();

        }
        else if (_tabIndex == 2)
        {
            //コピーボタンを表示
            if (GUILayout.Button("コピー"))
            {
                Debug.Log("START:コピー");
                //アバターが設定されているか確認
                if (AvatarObject == null)
                {
                    Debug.Log("Avatarを選択していません。");
                    resultMessage = "Avatarを選択していません。";
                }
                else
                {
                    Debug.Log(AvatarObject.name + "のLayerコピー開始。");
                    Undo.RecordObject(obj, "Clone Layer");
                    

                    CopyEXLayer();

                    EditorUtility.SetDirty(obj);
                    AssetDatabase.SaveAssets();
                    Debug.Log(AvatarObject.name + "のLayerコピー終了。");
                }
                Debug.Log("END:コピー");

            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("【Base】");
            BaseLayer_Copy = EditorGUILayout.Toggle("BaseLayer", BaseLayer_Copy);
            obj.BaseLayer_Copy = BaseLayer_Copy;

            AdditiveLayer_Copy = EditorGUILayout.Toggle("AdditiveLayer", AdditiveLayer_Copy);
            obj.AdditiveLayer_Copy = AdditiveLayer_Copy;

            GestureLayer_Copy = EditorGUILayout.Toggle("GestureLayer", GestureLayer_Copy);
            obj.GestureLayer_Copy = GestureLayer_Copy;

            ActionLayer_Copy = EditorGUILayout.Toggle("ActionLayer", ActionLayer_Copy);
            obj.ActionLayer_Copy = ActionLayer_Copy;

            FXLayer_Copy = EditorGUILayout.Toggle("FXLayer", FXLayer_Copy);
            obj.FXLayer_Copy = FXLayer_Copy;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("【Special】");
            SittngLayer_Copy = EditorGUILayout.Toggle("SittngLayer", SittngLayer_Copy);
            obj.SittngLayer_Copy = SittngLayer_Copy;

            TPoseLayer_Copy = EditorGUILayout.Toggle("TPoseLayer", TPoseLayer_Copy);
            obj.TPoseLayer_Copy = TPoseLayer_Copy;

            IKPoseLayer_Copy = EditorGUILayout.Toggle("IKPoseLayer", IKPoseLayer_Copy);
            obj.IKPoseLayer_Copy = IKPoseLayer_Copy;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("【Expressions】");
            EXMenu_Copy = EditorGUILayout.Toggle("EXMenu", EXMenu_Copy);
            obj.EXMenu_Copy = EXMenu_Copy;

            EXPara_Copy = EditorGUILayout.Toggle("EXPara", EXPara_Copy);
            obj.EXPara_Copy = EXPara_Copy;

            EditorGUILayout.EndVertical();
        }
    }

    private void CopyEXLayer()
    {
        Debug.Log("EXとLayerのコピー開始。");
        VRCAvatarDescriptor avatar = AvatarObject.GetComponent<VRCAvatarDescriptor>();
        string saveFolderPath = assetFolderPath + avatar.gameObject.name;

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
            AssetDatabase.Refresh();
        }


        UnityEditor.Animations.AnimatorController animatorController;

        if (BaseLayer_Copy && !avatar.baseAnimationLayers[AvatarUtils.BASELAYER].isDefault && avatar.baseAnimationLayers[AvatarUtils.BASELAYER].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimator(avatar, AvatarUtils.BASELAYER);
            Debug.Log("BaseLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/BaseLayer.controller");
            avatar.baseAnimationLayers[AvatarUtils.BASELAYER] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.Base
            };
            Debug.Log("BaseLayerコピー：" + saveFolderPath + "/BaseLayer.controller");
        }
        if (AdditiveLayer_Copy && !avatar.baseAnimationLayers[AvatarUtils.ADDITIVELAYER].isDefault && avatar.baseAnimationLayers[AvatarUtils.ADDITIVELAYER].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimator(avatar, AvatarUtils.ADDITIVELAYER);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/AdditiveLayer.controller");
            avatar.baseAnimationLayers[AvatarUtils.ADDITIVELAYER] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.Additive
            };
            Debug.Log("AdditiveLayerコピー：" + saveFolderPath + "/AdditiveLayer.controller");
        }
        if (GestureLayer_Copy && !avatar.baseAnimationLayers[AvatarUtils.GESTURELAYER].isDefault && avatar.baseAnimationLayers[AvatarUtils.GESTURELAYER].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimator(avatar, AvatarUtils.GESTURELAYER);
            Debug.Log("GestureLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/GestureLayer.controller");
            Debug.Log("GestureLayerコピー：" + saveFolderPath + "/GestureLayer.controller");
            if (animatorController != null)
            {
                avatar.baseAnimationLayers[AvatarUtils.GESTURELAYER] = new CustomAnimLayer()
                {
                    isEnabled = true,
                    animatorController = animatorController,
                    type = AnimLayerType.Gesture
                };
            }
            Debug.Log("GestureLayerコピー：" + saveFolderPath + "/GestureLayer.controller");
        }
        if (ActionLayer_Copy && !avatar.baseAnimationLayers[AvatarUtils.ACTIONLAYER].isDefault && avatar.baseAnimationLayers[AvatarUtils.ACTIONLAYER].animatorController != null)
        {

            animatorController = AvatarUtils.GetAnimator(avatar, AvatarUtils.ACTIONLAYER);
            Debug.Log("ActionLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/ActionLayer.controller");
            avatar.baseAnimationLayers[AvatarUtils.ACTIONLAYER] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.Action
            };
            Debug.Log("ActionLayerコピー：" + saveFolderPath + "/ActionLayer.controller");
        }
        if (FXLayer_Copy && !avatar.baseAnimationLayers[AvatarUtils.FXLAYER].isDefault && avatar.baseAnimationLayers[AvatarUtils.FXLAYER].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimator(avatar, AvatarUtils.FXLAYER);
            Debug.Log("FXLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/FXLayer.controller");
            avatar.baseAnimationLayers[AvatarUtils.FXLAYER] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.FX
            };
            Debug.Log("FXLayerコピー：" + saveFolderPath + "/FXLayer.controller");
        }
        
        //SittngLayerコピー
        if (SittngLayer_Copy && !avatar.specialAnimationLayers[AvatarUtils.SITTING].isDefault && avatar.specialAnimationLayers[AvatarUtils.SITTING].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimatorSpecaial(avatar, AvatarUtils.SITTING);
            Debug.Log("SittngLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/SittngLayer.controller");
            avatar.specialAnimationLayers[AvatarUtils.SITTING] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.Sitting
            };
            Debug.Log("SittngLayerコピー：" + saveFolderPath + "/SittngLayer.controller");
        }
        //TPoseLayerコピー
        if (TPoseLayer_Copy && !avatar.specialAnimationLayers[AvatarUtils.TPOSE].isDefault && avatar.specialAnimationLayers[AvatarUtils.TPOSE].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimatorSpecaial(avatar, AvatarUtils.TPOSE);
            Debug.Log("TPoseLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/TPoseLayer.controller");
            avatar.specialAnimationLayers[AvatarUtils.TPOSE] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.TPose
            };
            Debug.Log("TPoseLayerコピー：" + saveFolderPath + "/TPoseLayer.controller");
        }
        //IKPoseLayerコピー
        if (IKPoseLayer_Copy && !avatar.specialAnimationLayers[AvatarUtils.IKPOSE].isDefault && avatar.specialAnimationLayers[AvatarUtils.IKPOSE].animatorController != null)
        {
            animatorController = AvatarUtils.GetAnimatorSpecaial(avatar, AvatarUtils.IKPOSE);
            Debug.Log("IKPoseLayerコピー：" + animatorController.name);
            animatorController = AvatarUtils.CopyAnimator(animatorController, saveFolderPath + "/IKPoseLayer.controller");
            avatar.specialAnimationLayers[AvatarUtils.IKPOSE] = new CustomAnimLayer()
            {
                isEnabled = true,
                animatorController = animatorController,
                type = AnimLayerType.IKPose
            };
            Debug.Log("IKPoseLayerコピー：" + saveFolderPath + "/IKPoseLayer.controller");
        }

        if (EXMenu_Copy && avatar.expressionsMenu != null)
        {
            avatar.expressionsMenu = AvatarUtils.CopyExpMenu(avatar.expressionsMenu, saveFolderPath + "/ExMenu.asset");
            Debug.Log("expressionsMenuコピー" + saveFolderPath + "/ExMenu.asset");
        }
        if (EXPara_Copy && avatar.expressionParameters != null)
        {
            avatar.expressionParameters = AvatarUtils.CopyExpPara(avatar.expressionParameters, saveFolderPath + "/ExPara.asset");
            Debug.Log("expressionParametersコピー" + saveFolderPath + "/ExPara.asset");
        }
        AssetDatabase.Refresh();
        Debug.Log("EXとLayerのコピー終了。");
    }

    //アニメーションキー設定配列からアニメーションを作成
    private void LightSettingAdvancedApply(GameObject obj, List<KeySetActionData> keyActionList)
    {

        VRCAvatarDescriptor avatar = AvatarObject.GetComponent<VRCAvatarDescriptor>();
        VRCExpressionsMenu menu = AvatarUtils.GetExpMenu(avatar);
        VRCExpressionParameters parameters = AvatarUtils.GetExpPara(avatar);

        string LightAnimClipName = assetFolderPath + "LightAnimClip_" + AvatarObject.name + ".anim";

        string LightLimitsName = "LightLimits";
        string LightLimitsMenuName = "LightSettingALL";

        ExpressionsMenuSubParameter[] subParameter_LightLimits = new ExpressionsMenuSubParameter[1] { new ExpressionsMenuSubParameter { subParameterName = LightLimitsName, label = LightLimitsName } };

        //パラメータの追加チェック
        if ((AvatarUtils.CheckExpParaMultiple(parameters, VRCExpressionParameters.ValueType.Float, 1) == false
            && AvatarUtils.FindExpPara(parameters, LightLimitsName, VRCExpressionParameters.ValueType.Float) == null))
        {
            Debug.Log("パラメータが追加できません。");
            resultMessage = "ExpressionParameterにパラメータが追加できません。\r\n" + "不要なパラメータを削除してください。";
            return;
        }

        //メニューの追加チェック
        if (AvatarUtils.CheckMenuMultiple(menu, 1) == false
            && AvatarUtils.CheckExpSubMenu(menu, LightLimitsMenuName, VRCExpressionsMenu.Control.ControlType.SubMenu) == false)
        {
            Debug.Log("サブメニューが追加できません。");
            resultMessage = "ExpressionMenuにサブメニューが追加できません。\r\n" + "不要なメニューを削除してください。";
            return;
        }

        AnimationClip LightLimitsClip = AvatarUtils.CreateAnimationClip();

        AddAnimation(AvatarObject, LightLimitsClip, keyActionList);

        AvatarUtils.SaveAnimationClip(LightLimitsClip, LightAnimClipName);

        //アニメーター適用
        UnityEditor.Animations.AnimatorController controller = AvatarUtils.GetAnimator(avatar, AvatarUtils.FXLAYER);
        UnityEditor.Animations.AnimatorControllerLayer layer = AvatarUtils.CreateAnimatorLayer(controller, LightLimitsName, true);
        AvatarUtils.CreateAnimatorParameter(controller, LightLimitsName, UnityEngine.AnimatorControllerParameterType.Float);
        AvatarUtils.CreateAnimatorState(layer, LightLimitsName, LightLimitsClip, writeDefaultValues, LightLimitsName);


        //ExpressionParameters適用
        AvatarUtils.AddExpPara(parameters, LightLimitsName, VRCExpressionParameters.ValueType.Float, LightLimitsList_Save, LightLimitsList_Default);

        //ExpressionMenu適用
        VRCExpressionsMenu subMenu = AvatarUtils.CreateExpMenu(avatar, assetFolderPath, LightLimitsMenuName + ".asset");

        AvatarUtils.AddExpSubMenu(menu, LightLimitsMenuName, VRCExpressionsMenu.Control.ControlType.SubMenu, subMenu);
        AvatarUtils.AddExpMenu(subMenu, LightLimitsName, VRCExpressionsMenu.Control.ControlType.RadialPuppet, null, subParameter_LightLimits);

        //ExpressionParameters、ExpressionMenuを保存（20220318追加）
        EditorUtility.SetDirty(parameters);
        EditorUtility.SetDirty(menu);
        EditorUtility.SetDirty(subMenu);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        resultMessage = "正常終了";
    }

    //MaximumLightLimitsとMinimumLightLimitsをまとめて設定する
    private void LightSettingALLApply(GameObject obj)
    {

        VRCAvatarDescriptor avatar = AvatarObject.GetComponent<VRCAvatarDescriptor>();
        VRCExpressionsMenu menu = AvatarUtils.GetExpMenu(avatar);
        VRCExpressionParameters parameters = AvatarUtils.GetExpPara(avatar);

        string LightAnimClipName = assetFolderPath + "LightAnimClip_" + AvatarObject.name + ".anim";

        string LightLimitsName = "LightLimits";
        string LightLimitsMenuName = "LightSettingALL";

        ExpressionsMenuSubParameter[] subParameter_LightLimits = new ExpressionsMenuSubParameter[1] { new ExpressionsMenuSubParameter { subParameterName = LightLimitsName, label = LightLimitsName } };

        //パラメータの追加チェック
        if ((AvatarUtils.CheckExpParaMultiple(parameters, VRCExpressionParameters.ValueType.Float, 1) == false
            && AvatarUtils.FindExpPara(parameters, LightLimitsName, VRCExpressionParameters.ValueType.Float) == null))
        {
            Debug.Log("パラメータが追加できません。");
            resultMessage = "ExpressionParameterにパラメータが追加できません。\r\n" + "不要なパラメータを削除してください。";
            return;
        }
        
        //メニューの追加チェック
        if (AvatarUtils.CheckMenuMultiple(menu, 1) == false
            && AvatarUtils.CheckExpSubMenu(menu, LightLimitsMenuName, VRCExpressionsMenu.Control.ControlType.SubMenu) == false)
        {
            Debug.Log("サブメニューが追加できません。");
            resultMessage = "ExpressionMenuにサブメニューが追加できません。\r\n" + "不要なメニューを削除してください。";
            return;
        }

        AnimationClip LightLimitsClip = AvatarUtils.CreateAnimationClip();

        ApplyChildrenALL(AvatarObject, LightLimitsClip);

        AvatarUtils.SaveAnimationClip(LightLimitsClip, LightAnimClipName);

        //アニメーター適用
        UnityEditor.Animations.AnimatorController controller = AvatarUtils.GetAnimator(avatar, AvatarUtils.FXLAYER);
        UnityEditor.Animations.AnimatorControllerLayer layer = AvatarUtils.CreateAnimatorLayer(controller, LightLimitsName, true);
        AvatarUtils.CreateAnimatorParameter(controller, LightLimitsName, UnityEngine.AnimatorControllerParameterType.Float);
        AvatarUtils.CreateAnimatorState(layer, LightLimitsName, LightLimitsClip, writeDefaultValues, LightLimitsName);

        //ExpressionParameters適用
        AvatarUtils.AddExpPara(parameters, LightLimitsName, VRCExpressionParameters.ValueType.Float, LightLimits_Save, LightLimits_Default);

        //ExpressionMenu適用
        VRCExpressionsMenu subMenu = AvatarUtils.CreateExpMenu(avatar, assetFolderPath, LightLimitsMenuName + ".asset");

        AvatarUtils.AddExpSubMenu(menu, LightLimitsMenuName, VRCExpressionsMenu.Control.ControlType.SubMenu, subMenu);
        AvatarUtils.AddExpMenu(subMenu, LightLimitsName, VRCExpressionsMenu.Control.ControlType.RadialPuppet, null, subParameter_LightLimits);

        //ExpressionParameters、ExpressionMenuを保存（20220318追加）
        EditorUtility.SetDirty(parameters);
        EditorUtility.SetDirty(menu);
        EditorUtility.SetDirty(subMenu);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        resultMessage = "正常終了";
    }

    //MaximumLightLimitsとMinimumLightLimitsを個別に設定する
    private void LightSettingApply(GameObject obj)
    {

        VRCAvatarDescriptor avatar = AvatarObject.GetComponent<VRCAvatarDescriptor>();
        VRCExpressionsMenu menu = AvatarUtils.GetExpMenu(avatar);
        VRCExpressionParameters parameters = AvatarUtils.GetExpPara(avatar);

        string MinLightAnimClipName = assetFolderPath + "MinLightAnimClip_" + AvatarObject.name + ".anim";
        string MaxLightAnimClipName = assetFolderPath + "MaxLightAnimClip_" + AvatarObject.name + ".anim";

        string MinLightLimitsName = "MinLightLimits";
        string MaxLightLimitsName = "MaxLightLimits";

        ExpressionsMenuSubParameter[] subParameter_MinLightLimits = new ExpressionsMenuSubParameter[1] { new ExpressionsMenuSubParameter { subParameterName = MinLightLimitsName, label = MinLightLimitsName } };
        ExpressionsMenuSubParameter[] subParameter_MaxLightLimits = new ExpressionsMenuSubParameter[1] { new ExpressionsMenuSubParameter { subParameterName = MaxLightLimitsName, label = MaxLightLimitsName } };

        //パラメータの追加チェック
        if ((AvatarUtils.CheckExpParaMultiple(parameters, VRCExpressionParameters.ValueType.Float, 2) == false
            && AvatarUtils.FindExpPara(parameters, MinLightLimitsName, VRCExpressionParameters.ValueType.Float) == null
            && AvatarUtils.FindExpPara(parameters, MaxLightLimitsName, VRCExpressionParameters.ValueType.Float) == null))
        {
            Debug.Log("両方ない。");
            resultMessage = "ExpressionParameterにパラメータが追加できません。\r\n" + "不要なパラメータを削除してください。";
            return;
        }

        if ((AvatarUtils.CheckExpParaMultiple(parameters, VRCExpressionParameters.ValueType.Float, 1) == false
            && AvatarUtils.FindExpPara(parameters, MinLightLimitsName, VRCExpressionParameters.ValueType.Float) == null
            && AvatarUtils.FindExpPara(parameters, MaxLightLimitsName, VRCExpressionParameters.ValueType.Float) != null))
        {
            Debug.Log("Minがない。");
            resultMessage = "ExpressionParameterにパラメータが追加できません。\r\n" + "不要なパラメータを削除してください。";
            return;
        }

        if ((AvatarUtils.CheckExpParaMultiple(parameters, VRCExpressionParameters.ValueType.Float, 1) == false
            && AvatarUtils.FindExpPara(parameters, MinLightLimitsName, VRCExpressionParameters.ValueType.Float) != null
            && AvatarUtils.FindExpPara(parameters, MaxLightLimitsName, VRCExpressionParameters.ValueType.Float) == null))
        {
            Debug.Log("Maxがない。");
            resultMessage = "ExpressionParameterにパラメータが追加できません。\r\n" + "不要なパラメータを削除してください。";
            return;
        }

        //メニューの追加チェック
        if (AvatarUtils.CheckMenuMultiple(menu, 1) == false
            && AvatarUtils.CheckExpSubMenu(menu, "LightSetting", VRCExpressionsMenu.Control.ControlType.SubMenu) == false)
        {
            Debug.Log("サブメニューが追加できません。");
            resultMessage = "ExpressionMenuにサブメニューが追加できません。\r\n" + "不要なメニューを削除してください。";
            return;
        }

        AnimationClip MinLightLimitsClip = AvatarUtils.CreateAnimationClip();
        AnimationClip MaxLightLimitsClip = AvatarUtils.CreateAnimationClip();

        ApplyChildren(AvatarObject, MinLightLimitsClip, MaxLightLimitsClip);
        
        AvatarUtils.SaveAnimationClip(MinLightLimitsClip, MinLightAnimClipName);
        AvatarUtils.SaveAnimationClip(MaxLightLimitsClip, MaxLightAnimClipName);

        //アニメーター適用
        UnityEditor.Animations.AnimatorController controller = AvatarUtils.GetAnimator(avatar, AvatarUtils.FXLAYER);
        UnityEditor.Animations.AnimatorControllerLayer layer = AvatarUtils.CreateAnimatorLayer(controller, MinLightLimitsName,true);
        AvatarUtils.CreateAnimatorParameter(controller, MinLightLimitsName, UnityEngine.AnimatorControllerParameterType.Float);
        AvatarUtils.CreateAnimatorState(layer, MinLightLimitsName, MinLightLimitsClip, writeDefaultValues, MinLightLimitsName);

        layer = AvatarUtils.CreateAnimatorLayer(controller, MaxLightLimitsName,true);
        AvatarUtils.CreateAnimatorParameter(controller, MaxLightLimitsName, UnityEngine.AnimatorControllerParameterType.Float);
        AvatarUtils.CreateAnimatorState(layer, MaxLightLimitsName, MaxLightLimitsClip, writeDefaultValues, MaxLightLimitsName);

        //ExpressionParameters適用
        AvatarUtils.AddExpPara(parameters, MinLightLimitsName, VRCExpressionParameters.ValueType.Float, MinLightLimits_Save, MinLightLimits_Default);
        AvatarUtils.AddExpPara(parameters, MaxLightLimitsName, VRCExpressionParameters.ValueType.Float, MaxLightLimits_Save, MaxLightLimits_Default);

        //ExpressionMenu適用
        VRCExpressionsMenu subMenu = AvatarUtils.CreateExpMenu(avatar, assetFolderPath, "LightSetting.asset");

        AvatarUtils.AddExpSubMenu(menu, "LightSetting", VRCExpressionsMenu.Control.ControlType.SubMenu, subMenu);
        AvatarUtils.AddExpMenu(subMenu, MinLightLimitsName, VRCExpressionsMenu.Control.ControlType.RadialPuppet, null, subParameter_MinLightLimits);
        AvatarUtils.AddExpMenu(subMenu, MaxLightLimitsName, VRCExpressionsMenu.Control.ControlType.RadialPuppet, null, subParameter_MaxLightLimits);

        //ExpressionParameters、ExpressionMenuを保存（20220318追加）
        EditorUtility.SetDirty(parameters);
        EditorUtility.SetDirty(menu);
        EditorUtility.SetDirty(subMenu);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        resultMessage = "正常終了";
    }

    //アバターの子オブジェクトを再帰ループして、Unlit,MaximumLightLimits、MinimumLightLimitsをアニメーションクリップに追加
    private void AddAnimation(GameObject obj, AnimationClip LightLimitsClip, List<KeySetActionData> keyActionList)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        string path = "";
        id = serializedObject.FindProperty("id");
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }
         
        //アニメーション作成
        foreach (Transform ob in children)
        {
            path = Extensions.GetFullPath(ob.gameObject).Replace(GetFullPath(AvatarObject) + "/", "");
            Debug.Log(AvatarObject.name + "/");
            Debug.Log("path:" + path);
            foreach (KeySetActionData keyAction in keyActionList)
            {
                AddAnimationKey(ob.gameObject, LightLimitsClip, keyAction.percent, keyAction.unlit_value, path, shaderkeyword_AsUnlit);
                AddAnimationKey(ob.gameObject, LightLimitsClip, keyAction.percent, keyAction.minLimits_value, path, shaderkeyword_LightMinLimit);
                AddAnimationKey(ob.gameObject, LightLimitsClip, keyAction.percent, keyAction.maxLimits_value, path, shaderkeyword_LightMaxLimit);
            }

            AddAnimation(ob.gameObject, LightLimitsClip, keyActionList);
        }
    }

    //アバターの子オブジェクトを再帰ループして、MaximumLightLimitsとMinimumLightLimitsをまとめて設定する
    private void ApplyChildrenALL(GameObject obj, AnimationClip LightLimitsClip)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        string path = "";
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }

        //アニメーション作成
        foreach (Transform ob in children)
        {
            path = Extensions.GetFullPath(ob.gameObject).Replace(GetFullPath(AvatarObject) + "/", "");
            Debug.Log(AvatarObject.name + "/");
            Debug.Log("path:" + path);

            AddAnimationKey(ob.gameObject, LightLimitsClip, 0f, MinLightLimits_Min, path, shaderkeyword_LightMinLimit);
            AddAnimationKey(ob.gameObject, LightLimitsClip, 1f, MinLightLimits_Max, path, shaderkeyword_LightMinLimit);

            AddAnimationKey(ob.gameObject, LightLimitsClip, 0f, MaxLightLimits_Min, path, shaderkeyword_LightMaxLimit);
            AddAnimationKey(ob.gameObject, LightLimitsClip, 1f, MaxLightLimits_Max, path, shaderkeyword_LightMaxLimit);

            ApplyChildrenALL(ob.gameObject, LightLimitsClip);
        }
    }

    //アバターの子オブジェクトを再帰ループして、MaximumLightLimitsとMinimumLightLimitsを個別設定する
    private void ApplyChildren(GameObject obj, AnimationClip MinLightLimitsClip, AnimationClip MaxLightLimitsClip)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        string path = "";
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }

        //アニメーション作成
        foreach (Transform ob in children)
        {
            path = Extensions.GetFullPath(ob.gameObject).Replace(GetFullPath(AvatarObject) + "/", "");
            Debug.Log(AvatarObject.name + "/");
            Debug.Log("path:" + path);
            AddAnimationKey(ob.gameObject, MinLightLimitsClip, 0f, MinLightLimits_Min, path, shaderkeyword_LightMinLimit);
            AddAnimationKey(ob.gameObject, MinLightLimitsClip, 1f, MinLightLimits_Max, path, shaderkeyword_LightMinLimit);
            
            AddAnimationKey(ob.gameObject, MaxLightLimitsClip, 0f, MaxLightLimits_Min, path, shaderkeyword_LightMaxLimit);
            AddAnimationKey(ob.gameObject, MaxLightLimitsClip, 1f, MaxLightLimits_Max, path, shaderkeyword_LightMaxLimit);

            ApplyChildren(ob.gameObject, MinLightLimitsClip, MaxLightLimitsClip);
        }
    }

    //アニメーションにキーを追加する
    private void AddAnimationKey(GameObject go, AnimationClip clip, float time, float value,string path, string shaderkeyword)
    {
        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();

        if (smr != null)
        {
            Undo.RecordObject(smr, "Add Animation -"+ shaderkeyword + "-");

            foreach (var mat in smr.sharedMaterials)
            {
                //マテリアル未設定時にエラーが出ていたので対応(20220627)
                //motchiriシェーダー対応（20220927）
                if (mat != null)
                {
                    Debug.Log(mat.shader.name);
                    if (mat.shader.name.IndexOf("lilToon") >= 0 || mat.shader.name.IndexOf("motchiri") >= 0)
                    {
                        //アニメーション作成
                        AvatarUtils.AddAnimation(clip, time * 0.01f, value, shaderkeyword, path, typeof(SkinnedMeshRenderer));
                    }
                }
                    
            }
        }

        MeshRenderer mr = go.GetComponent<MeshRenderer>();

        if (mr != null)
        {
            Undo.RecordObject(mr, "Add Animation -" + shaderkeyword + "-");

            foreach (var mat in mr.sharedMaterials)
            {
                //マテリアル未設定時にエラーが出ていたので対応(20220627)
                //motchiriシェーダー対応（20220927）
                if (mat != null)
                {
                    Debug.Log(mat.shader.name);
                    if (mat.shader.name.IndexOf("lilToon") >= 0 || mat.shader.name.IndexOf("motchiri") >= 0)
                    {
                        //アニメーション作成
                        AvatarUtils.AddAnimation(clip, time * 0.01f, value, shaderkeyword, path, typeof(MeshRenderer));
                    }
                }
            }
        }
    }

    public string GetFullPath(GameObject gameObject)
    {
        Transform transform = gameObject.transform;
        string path = transform.name;
        var parent = transform.parent;
        while (parent)
        {
            path = $"{parent.name}/{path}";
            parent = parent.parent;
        }
        return path;
    }

}
