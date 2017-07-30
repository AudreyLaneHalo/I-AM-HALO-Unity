using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace MalbersAnimations
{
    [CustomEditor(typeof(ActionZone))]
    public class ActionZoneEditor : Editor
    {
        private ActionZone mActionZone;
      //  private bool swap;

        string[] actionNames;

        private void OnEnable()
        {
            mActionZone = ((ActionZone)target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Actions && Emotions for activating the Zones ", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            mActionZone.actionsToUse = (Actions)EditorGUILayout.ObjectField("Actions Pack to use", mActionZone.actionsToUse, typeof(Actions), false);

            if (mActionZone.actionsToUse != null)
            {
                actionNames = new string[mActionZone.actionsToUse.actions.Length];
                for (int i = 0; i < mActionZone.actionsToUse.actions.Length; i++)
                {
                    actionNames[i] = mActionZone.actionsToUse.actions[i].name;
                }
                mActionZone.index = EditorGUILayout.Popup("Actions & Emotions", mActionZone.index, actionNames);
                mActionZone.ID = mActionZone.actionsToUse.actions[mActionZone.index].ID;
            }
            else
            {
                EditorGUILayout.HelpBox("Add an Actions & Emotions Pack", MessageType.Warning);
            }

            mActionZone.HeadOnly = EditorGUILayout.Toggle(new GUIContent("Head Only", "Enable only when the 'Head' bone enter the trigger zone"), mActionZone.HeadOnly);
            mActionZone.automatic = EditorGUILayout.Toggle(new GUIContent("Automatic", "As soon as the animal enters the zone play the action"), mActionZone.automatic);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}