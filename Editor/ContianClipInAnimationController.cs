﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using AnimatorController = UnityEditor.Animations.AnimatorController;

public class ContianClipInAnimationController : EditorWindow
{
    private AnimatorController controller;

    string clipName;

    [MenuItem("Assets/Manage Animation Clip Under Animation Controller")]
    static void Create()
    {
        var window = ContianClipInAnimationController.GetWindow(typeof(ContianClipInAnimationController)) as ContianClipInAnimationController;
        if (Selection.activeObject is AnimatorController)
            window.controller = Selection.activeObject as AnimatorController;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Target clip");
        controller = EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false) as AnimatorController;

        if (controller == null)
            return;

        List<AnimationClip> clipList = new List<AnimationClip>();

        var allAsset = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(controller));
        foreach (var asset in allAsset)
        {
            if (asset is AnimationClip)
            {
                var removeClip = asset as AnimationClip;
                if (!clipList.Contains(removeClip))
                {
                    clipList.Add(removeClip);
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add new clip");
        EditorGUILayout.BeginVertical("box");

        clipName = EditorGUILayout.TextField(clipName);

        if (clipList.Exists(item => item.name == clipName) || string.IsNullOrEmpty(clipName))
        {
            EditorGUILayout.LabelField("Can't create duplicate names or empty");
        }
        else
        {
            if (GUILayout.Button("Create"))
            {
                AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(clipName);
                AssetDatabase.AddObjectToAsset(animationClip, controller);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(controller));
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.EndVertical();



        if (clipList.Count == 0)
            return;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Remove clip");
        EditorGUILayout.BeginVertical("box");

        foreach (var removeClip in clipList)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(removeClip.name);
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                Object.DestroyImmediate(removeClip, true);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(controller));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

    }
}
