﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;


namespace BBItems.AvatarUtils
{
    public struct ExpressionsMenuSubParameter
    {
        public string subParameterName;
        public string label;
        public Texture2D icon;
    }

    public static class AvatarUtils
    {
        public const int BASELAYER = 0;
        public const int ADDITIVELAYER = 1;
        public const int GESTURELAYER = 2;
        public const int ACTIONLAYER = 3;
        public const int FXLAYER = 4;

        public const int SITTING = 0;
        public const int TPOSE = 1;
        public const int IKPOSE = 2;

        //アニメータートランジション作成
        public static  void CreateAnimatorTransion(UnityEditor.Animations.AnimatorState fromState, UnityEditor.Animations.AnimatorState toState)
        {
            AnimatorStateTransition animatorTransition = fromState.AddTransition(toState);
        }

        //アニメーターステート取得
        public static UnityEditor.Animations.AnimatorState GetAnimatorState(AnimatorControllerLayer layer, string stateName)
        {
            UnityEditor.Animations.AnimatorState getState = null;
            foreach (var state in layer.stateMachine.states)
            {
                if(state.state.name == stateName)
                {
                    getState = state.state;
                }
            }

            return getState;
        }

        //アニメーターステート作成
        public static UnityEditor.Animations.AnimatorState CreateAnimatorState(AnimatorControllerLayer layer, string stateName, AnimationClip clip, bool writeDefaultValues, string timeParameterActive = "")
        {
            UnityEditor.Animations.AnimatorState state = GetAnimatorState(layer, stateName);
            if(state == null)
            {
                state = layer.stateMachine.AddState(stateName);
                state.name = stateName;
                state.motion = clip;
                if(timeParameterActive != "")
                {
                    state.timeParameterActive = true;
                    state.timeParameter = timeParameterActive;
                }

                state.writeDefaultValues = writeDefaultValues;
            }
            EditorUtility.SetDirty(state);
            return state;
        }

        //アニメーターパラメータ取得
        public static AnimatorControllerParameter GetAnimatorParameter(AnimatorController controller, string parameterName)
        {
            AnimatorControllerParameter getparameter = null;
            foreach (var parameter in controller.parameters)
            {
                if (parameter.name == parameterName)
                {
                    getparameter = parameter;
                    break;
                }
            }

            return getparameter;
        }

        //アニメーターパラメータ作成
        public static void CreateAnimatorParameter(AnimatorController controller, string parameterName, AnimatorControllerParameterType parameterType)
        {
            AnimatorControllerParameter parameter = GetAnimatorParameter(controller, parameterName);
            if(parameter == null)
            {
                parameter = new AnimatorControllerParameter();
                parameter.name = parameterName;
                parameter.type = parameterType;

                controller.AddParameter(parameter);
            }
        }

        //アニメーターパラメータ削除
        public static void DelAnimatorParameter(AnimatorController controller, string parameterName)
        {
            for (int i = 0; i < controller.parameters.Length; i++)
            {
                if (controller.parameters[i].name == parameterName)
                {

                    controller.RemoveParameter(i);
                }
            }
        }

        //アニメーターレイヤー取得
        public static AnimatorControllerLayer GetAnimatorLayer(AnimatorController controller, string layerName)
        {
            AnimatorControllerLayer getlayer = null;
            foreach(var layer in controller.layers)
            {
                if(layer.name == layerName)
                {
                    getlayer = layer;
                    break;
                }
            }

            return getlayer;
        }

        //アニメーターレイヤー作成
        public static AnimatorControllerLayer CreateAnimatorLayer(AnimatorController controller, string layerName, bool reCreate = false)
        {
            AnimatorControllerLayer layer = GetAnimatorLayer(controller, layerName);

            if (layer == null)
            {
                layer = new AnimatorControllerLayer();
                layer.name = layerName;
                layer.defaultWeight = 1;
                layer.blendingMode = AnimatorLayerBlendingMode.Override;
                layer.stateMachine = new AnimatorStateMachine();

                controller.AddLayer(layer);

                if (!AssetDatabase.IsSubAsset(layer.stateMachine))
                {
                    AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return layer;
            }
            if(reCreate == true)
            {
                DelAnimatorLayer(controller, layerName);
                
                layer.name = layerName;
                layer.defaultWeight = 1;
                layer.blendingMode = AnimatorLayerBlendingMode.Override;
                layer.stateMachine = new AnimatorStateMachine();

                controller.AddLayer(layer);
                
                if (!AssetDatabase.IsSubAsset(layer.stateMachine))
                {
                    AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return layer;
            }
            else
            {
                return layer;
            }
        }

        //アニメーターレイヤー削除
        public static void DelAnimatorLayer(AnimatorController controller, string layerName)
        {
            for(int i = 0; i < controller.layers.Length; i++)
            {
                if (controller.layers[i].name == layerName)
                {

                    controller.RemoveLayer(i);
                }
            }
        }

        //アニメーター取得（ベース用）
        public static AnimatorController GetAnimator(VRCAvatarDescriptor avatar, int AnimatorNumber)
        {
            if (avatar.baseAnimationLayers != null && avatar.baseAnimationLayers.Length >= 5 && avatar.baseAnimationLayers[AnimatorNumber].animatorController != null)
            {
                return (AnimatorController)avatar.baseAnimationLayers[AnimatorNumber].animatorController;
            }
            else
            {
                return null;
            }
        }

        //アニメーター取得（スペシャル用）
        public static AnimatorController GetAnimatorSpecaial(VRCAvatarDescriptor avatar, int AnimatorNumber)
        {
            if (avatar.specialAnimationLayers != null && avatar.specialAnimationLayers.Length >= 3 && avatar.specialAnimationLayers[AnimatorNumber].animatorController != null)
            {
                return (AnimatorController)avatar.specialAnimationLayers[AnimatorNumber].animatorController;
            }
            else
            {
                return null;
            }
        }

        //アニメーター作成
        public static AnimatorController CreateAnimator(VRCAvatarDescriptor avatar, string savePath, string AnimatorName, int AnimatorNumber)
        {
            var path = savePath + AnimatorName;
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

            DeleteAnimator(avatar, savePath, AnimatorName);
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            
            if (AnimatorNumber == 0)
            {
                avatar.baseAnimationLayers[0] = new CustomAnimLayer() { isEnabled = true, animatorController = controller, type = AnimLayerType.Base };
            }
            else if (AnimatorNumber == 1)
            {
                avatar.baseAnimationLayers[1] = new CustomAnimLayer() { isEnabled = true, animatorController = controller, type = AnimLayerType.Additive };
            }
            else if (AnimatorNumber == 2)
            {
                avatar.baseAnimationLayers[2] = new CustomAnimLayer() { isEnabled = true, animatorController = controller, type = AnimLayerType.Gesture };
            }
            else if (AnimatorNumber == 3)
            {
                avatar.baseAnimationLayers[3] = new CustomAnimLayer() { isEnabled = true, animatorController = controller, type = AnimLayerType.Action };
            }
            else if (AnimatorNumber == 4)
            {
                avatar.baseAnimationLayers[4] = new CustomAnimLayer() { isEnabled = true, animatorController = controller, type = AnimLayerType.FX };
            }

            return controller;

        }

        //アニメーター削除
        public static void DeleteAnimator(VRCAvatarDescriptor avatar, string savePath, string AnimatorName, int AnimatorNumber = -1)
        {
            var path = savePath + AnimatorName;
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

            if (asset != null)
            {
                AssetDatabase.DeleteAsset(path);
            }
            if (AnimatorNumber == -1)
            {

            }
            else if (AnimatorNumber == 0)
            {
                avatar.baseAnimationLayers[0] = new CustomAnimLayer() { isEnabled = false};
            }
            else if (AnimatorNumber == 1)
            {
                avatar.baseAnimationLayers[1] = new CustomAnimLayer() { isEnabled = false };
            }
            else if (AnimatorNumber == 2)
            {
                avatar.baseAnimationLayers[2] = new CustomAnimLayer() { isEnabled = false };
            }
            else if (AnimatorNumber == 3)
            {
                avatar.baseAnimationLayers[3] = new CustomAnimLayer() { isEnabled = false };
            }
            else if (AnimatorNumber == 4)
            {
                avatar.baseAnimationLayers[4] = new CustomAnimLayer() { isEnabled = false };
            }
        }

        //アニメーション作成
        public static AnimationClip CreateAnimationClip()
        {
            return new AnimationClip();
        }

        //アニメーション削除
        public static void DelAnimationClip(string savePath)
        {
            if (System.IO.File.Exists(savePath))
            {
                AssetDatabase.DeleteAsset(savePath);
            }
        }

        //アニメーション保存
        public static void SaveAnimationClip(AnimationClip clip, string savePath)
        {
            DelAnimationClip(savePath);

            Debug.Log(savePath);
            AssetDatabase.CreateAsset(
                clip,
                AssetDatabase.GenerateUniqueAssetPath(savePath)
            );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //アニメーションにキーを追加
        public static void AddAnimation(AnimationClip clip, float time, float value, string propertyName, string path,Type type)
        {
            EditorCurveBinding curveBinding;
            AnimationCurve curve;
            bool complete = false;

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (binding.propertyName.StartsWith(propertyName) && binding.path == path)
                {
                    for (int j = 0; j < curve.keys.Length; j++)
                    {
                        curve.AddKey(time, value);
                        AnimationUtility.SetEditorCurve(clip, binding, curve);
                        return;
                    }
                }
            }

            if(complete == false)
            {
                curveBinding = new EditorCurveBinding();
                curveBinding.path = path;
                curveBinding.type = type;
                curveBinding.propertyName = propertyName;

                curve = new AnimationCurve();
                curve.AddKey(time, value);
                AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
            }
        }

        //アニメーションからキー削除
        public static void DelAnimation(AnimationClip clip, string propertyName)
        {
            bool FlgRetry = true;
            AnimationClip newClip = CreateAnimationClip();

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (binding.propertyName.StartsWith(propertyName))
                {
                    Debug.Log(curve.keys.Length);
                    while(FlgRetry)
                    {
                        FlgRetry = false;
                        for (int j = 0; j < curve.keys.Length; j++)
                        {
                            curve.RemoveKey(j);
                            AnimationUtility.SetEditorCurve(clip, binding, curve);
                            FlgRetry = true;
                        }
                    }
                }
            }
        }

        //ExpressionParameters作成
        public static VRCExpressionParameters CreateExpPara(VRCAvatarDescriptor avatar, string createFolder)
        {
            var para = ScriptableObject.CreateInstance<VRCExpressionParameters>();
            para.parameters = new VRCExpressionParameters.Parameter[0];
            var paraPath = createFolder + "ExpressionParameters.asset";
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(paraPath);
            if(asset != null)
            {
                AssetDatabase.DeleteAsset(paraPath);
            }
            AssetDatabase.CreateAsset(para, paraPath);
            avatar.expressionParameters = para;

            return para;
        }

        //ExpressionParameters取得
        public static VRCExpressionParameters GetExpPara(VRCAvatarDescriptor avatar)
        {
            if(avatar.expressionParameters != null)
            {
                return avatar.expressionParameters;
            }
            else
            {
                return null;
            }
        }

        //ExpressionParametersにパラメータカウントチェック
        public static bool CheckExpParaMultiple(VRCExpressionParameters exPara, VRCExpressionParameters.ValueType valueType, int count)
        {
            int currentCost = exPara.CalcTotalCost();
            currentCost += VRCExpressionParameters.TypeCost(valueType) * count;

            if (currentCost <= VRCExpressionParameters.MAX_PARAMETER_COST)
            {
                return true;
            }
            return false;
        }

        //ExpressionParametersにパラメータカウントチェック
        public static bool CheckExpParaAdd(VRCExpressionParameters exPara, string name, VRCExpressionParameters.ValueType valueType, bool saved = false, float defaultValue = 0f)
        {
            int currentCost = exPara.CalcTotalCost();
            var hasParam = exPara.parameters.Any(n => n.name == name && n.valueType == valueType);
            if(!hasParam)
            {
                currentCost += VRCExpressionParameters.TypeCost(valueType);
            }

            if(currentCost <= VRCExpressionParameters.MAX_PARAMETER_COST)
            {
                return true;
            }
            return false;
        }

        //ExpressionParametersにパラメータ追加
        public static bool AddExpPara(VRCExpressionParameters exPara, string name, VRCExpressionParameters.ValueType valueType, bool saved = false, float defaultValue = 0f)
        {
            VRCExpressionParameters.Parameter parameter = FindExpPara(exPara, name, valueType);
            if (parameter != null)
            {
                parameter.saved = saved;
                parameter.defaultValue = defaultValue;
                return true;
            }
            else if(CheckExpParaAdd(exPara, name, valueType, saved, defaultValue) && parameter == null)
            {
                var len = exPara.parameters.Length + 1;
                Array.Resize(ref exPara.parameters, len);
                exPara.parameters[len - 1] = new VRCExpressionParameters.Parameter() { name = name, valueType = valueType, saved = saved, defaultValue = defaultValue };
                return true;
            }
            else
            {
                Debug.Log("パラメータの追加に失敗しました。");
                return false;
            }
        }

        //ExpressionParametersのパラメータ削除
        public static void TryRemoveExpPara(VRCExpressionParameters exPara, string name, VRCExpressionParameters.ValueType valueType)
        {
            if(FindExpPara(exPara, name, valueType) != null)
            {
                exPara.parameters = exPara.parameters.Where(n => n.name != name).ToArray();
            }
        }

        //ExpressionParametersのパラメータ取得
        public static VRCExpressionParameters.Parameter FindExpPara(VRCExpressionParameters exPara, string name, VRCExpressionParameters.ValueType valueType)
        {
            return exPara.parameters.FirstOrDefault(n => n.name == name && n.valueType == valueType);
        }

        //ExpressionMenu作成
        public static VRCExpressionsMenu CreateExpMenu(VRCAvatarDescriptor avatar, string createFolder, string saveName, bool defaultMenu = false)
        {
            var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            var paraPath = createFolder + saveName;
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(paraPath);
            if (asset != null)
            {
                AssetDatabase.DeleteAsset(paraPath);
            }
            AssetDatabase.CreateAsset(menu, paraPath);
            if (defaultMenu == true)
            {
                avatar.expressionsMenu = menu;
            }
            return menu;
        }

        //ExpressionMenu取得
        public static VRCExpressionsMenu GetExpMenu(VRCAvatarDescriptor avatar)
        {
            if (avatar.expressionsMenu != null)
            {
                return avatar.expressionsMenu;
            }
            else
            {
                return null;
            }
        }

        //ExpressionMenu確認
        public static bool CheckMenuMultiple(VRCExpressionsMenu exMenu, int count)
        {
            if(exMenu.controls.Capacity + count >= 9)
            {
                Debug.Log("false");
                return false;
            }
            else
            {
                Debug.Log("true");
                return true;
            }
        }

        //ExpressionMenuにサブメニュー追加
        public static bool CheckExpSubMenu(VRCExpressionsMenu exMenu, string name, VRCExpressionsMenu.Control.ControlType controlType)
        {
            bool check = false;
            var control = new VRCExpressionsMenu.Control();
            control.name = name;
            control.type = controlType;

            foreach (var menuControl in exMenu.controls)
            {
                if (menuControl.name == control.name
                    && menuControl.type == control.type)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        //ExpressionMenu確認
        public static bool CheckExpMenu(VRCExpressionsMenu exMenu, string name, VRCExpressionsMenu.Control.ControlType controlType, string parameterName, ExpressionsMenuSubParameter[] subParameters)
        {
            bool check = false;
            var control = new VRCExpressionsMenu.Control();
            control.name = name;
            control.type = controlType;
            control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterName };

            if (subParameters.Length == 0)
            {

            }
            else if (subParameters.Length == 1)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[1] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName } };

                control.labels = new VRCExpressionsMenu.Control.Label[1] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon } };
            }
            else if (subParameters.Length == 2)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[2] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[1].subParameterName } };
                control.labels = new VRCExpressionsMenu.Control.Label[2] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[1].label, icon = subParameters[1].icon } };
            }
            else if (subParameters.Length == 3)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[3] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[1].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[2].subParameterName }};
                control.labels = new VRCExpressionsMenu.Control.Label[3] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[1].label, icon = subParameters[1].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[2].label, icon = subParameters[2].icon } };
            }
            else if (subParameters.Length == 4)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[4] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[1].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[2].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[3].subParameterName }};
                control.labels = new VRCExpressionsMenu.Control.Label[4] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[1].label, icon = subParameters[1].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[2].label, icon = subParameters[2].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[3].label, icon = subParameters[3].icon }};

            }

            foreach(var menuControl in exMenu.controls)
            {
                if(menuControl.icon == control.icon
                    && menuControl.labels == control.labels
                    && menuControl.name == control.name
                    && menuControl.parameter == control.parameter
                    && menuControl.subMenu == control.subMenu
                    && menuControl.subParameters == control.subParameters
                    && menuControl.type == control.type
                    && menuControl.value == control.value)
                {
                    check = true;
                    break;
                }
                Debug.Log(menuControl.name);
            }
            Debug.Log("チェック");
            return check;
        }

        //ExpressionMenuにサブメニュー追加
        public static void AddExpSubMenu(VRCExpressionsMenu exMenu, string name, VRCExpressionsMenu.Control.ControlType controlType, VRCExpressionsMenu subMenu)
        {
            var control = new VRCExpressionsMenu.Control();
            control.name = name;
            control.type = controlType;
            control.subMenu = subMenu;

            foreach (var menuControl in exMenu.controls)
            {
                if (menuControl.name == control.name)
                {
                    exMenu.controls.Remove(menuControl);
                    break;
                }
                Debug.Log(menuControl.name);
            }

            exMenu.controls.Add(control);
        }

        //ExpressionMenu追加
        public static void AddExpMenu(VRCExpressionsMenu exMenu, string name, VRCExpressionsMenu.Control.ControlType controlType, string parameterName,ExpressionsMenuSubParameter[] subParameters)
        {
            var control = new VRCExpressionsMenu.Control();
            control.name = name;
            control.type = controlType;
            control.parameter = new VRCExpressionsMenu.Control.Parameter() { name = parameterName };
            
            if(subParameters.Length == 0)
            {
                
            }
            else if (subParameters.Length == 1)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[1] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName }};

                control.labels = new VRCExpressionsMenu.Control.Label[1] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon } };
            }
            else if(subParameters.Length == 2)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[2] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[1].subParameterName } };
                control.labels = new VRCExpressionsMenu.Control.Label[2] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[1].label, icon = subParameters[1].icon } };
            }
            else if (subParameters.Length == 3)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[3] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[1].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[2].subParameterName }};
                control.labels = new VRCExpressionsMenu.Control.Label[3] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[1].label, icon = subParameters[1].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[2].label, icon = subParameters[2].icon } };
            }
            else if (subParameters.Length == 4)
            {
                control.subParameters = new VRCExpressionsMenu.Control.Parameter[4] { new VRCExpressionsMenu.Control.Parameter() { name = subParameters[0].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[1].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[2].subParameterName },
                                                                                        new VRCExpressionsMenu.Control.Parameter() { name = subParameters[3].subParameterName }};
                control.labels = new VRCExpressionsMenu.Control.Label[4] { new VRCExpressionsMenu.Control.Label() { name = subParameters[0].label, icon = subParameters[0].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[1].label, icon = subParameters[1].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[2].label, icon = subParameters[2].icon },
                                                                            new VRCExpressionsMenu.Control.Label() { name = subParameters[3].label, icon = subParameters[3].icon }};

            }
            exMenu.controls.Add(control);
        }

        //アニメーターコピー
        public static AnimatorController CopyAnimator(AnimatorController controller, string savePath)
        {
            var path = AssetDatabase.GetAssetPath(controller);
            if (path != savePath)
            {
                AssetDatabase.CopyAsset(path, savePath);

                controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(savePath);

            }

            return controller;

        }
        //ExpressionMenuコピー
        public static VRCExpressionsMenu CopyExpMenu(VRCExpressionsMenu menu, string savePath)
        {
            var path = AssetDatabase.GetAssetPath(menu);

            if (path != savePath)
            {
                AssetDatabase.CopyAsset(path, savePath);

                menu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(savePath);

            }

            return menu;
        }
        //ExpressionParametersコピー
        public static VRCExpressionParameters CopyExpPara(VRCExpressionParameters para, string savePath)
        {
            var path = AssetDatabase.GetAssetPath(para);
            if (path != savePath)
            {
                AssetDatabase.CopyAsset(path, savePath);

                para = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(savePath);

            }

            return para;
        }
    }

    public static class Extensions
    {
        public static string GetFullPath(this GameObject obj)
        {
            return GetFullPath(obj.transform);
        }

        public static string GetFullPath(this Transform t)
        {
            string path = t.name;
            var parent = t.parent;
            while (parent)
            {
                path = $"{parent.name}/{path}";
                parent = parent.parent;
            }
            return path;
        }
    }
}

