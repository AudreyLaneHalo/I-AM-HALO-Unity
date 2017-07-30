﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(MaterialChanger))]
    public class MaterialChangerEditor : Editor
    {
        private ReorderableList list;
        private SerializedProperty materialList;
        private MaterialChanger MyMaterialChanger;

        private void OnEnable()
        {
            MyMaterialChanger = ((MaterialChanger)target);

            materialList = serializedObject.FindProperty("materialList");

            list = new ReorderableList(serializedObject, materialList, true, true, true, true);
            list.drawElementCallback = DrawElementCallback;
            list.drawHeaderCallback = HeaderCallbackDelegate;
            list.onAddCallback = OnAddCallBack;
        }

        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            Rect R_1 = new Rect(rect.x + 14, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 14 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2), EditorGUIUtility.singleLineHeight);
            MyMaterialChanger.showMeshesList = EditorGUI.ToggleLeft(R_0, "", MyMaterialChanger.showMeshesList);
            EditorGUI.LabelField(R_1, "Materials List", EditorStyles.miniLabel);
            EditorGUI.LabelField(R_2, "CURRENT", EditorStyles.centeredGreyMiniLabel);
        }

        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MyMaterialChanger.materialList[index];
            rect.y += 2;

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width - 65) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 14 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2), EditorGUIUtility.singleLineHeight);

                element.Name = EditorGUI.TextField(R_1, element.Name, EditorStyles.label);
            string buttonCap = "None";

            if (element.mesh != null)
            {
                EditorGUI.BeginDisabledGroup(!element.mesh.gameObject.activeSelf || element.materials.Length ==0);

                    if (element.materials.Length > element.current)
                    {
                        buttonCap = element.mesh.gameObject.activeSelf ? (element.materials[element.current] == null ? "None" : element.materials[element.current].name) : "Is Hidden";
                    }

                    if (GUI.Button(R_2, buttonCap, EditorStyles.miniButton))
                    {
                        ToggleButton(index);
                    }
                EditorGUI.EndDisabledGroup();
            }
        }

        void ToggleButton(int index)
        {
            if (MyMaterialChanger.materialList[index].mesh != null)
            {
                MyMaterialChanger.materialList[index].ChangeMaterial();
            }
        }



        void OnAddCallBack(ReorderableList list)
        {
            if (MyMaterialChanger.materialList == null)
            {
                MyMaterialChanger.materialList = new System.Collections.Generic.List<MaterialItem>();
            }
            MyMaterialChanger.materialList.Add(new MaterialItem());
        }


     

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Swap Materials", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                list.DoLayoutList();
                EditorGUI.indentLevel++;

                if (MyMaterialChanger.showMeshesList)
                {
                    if (list.index != -1)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        SerializedProperty Element = materialList.GetArrayElementAtIndex(list.index);
                        EditorGUILayout.PropertyField(Element, true);
                        EditorGUILayout.EndVertical();
                    }
                }


                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}